namespace Indabo.Host
{
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    using Indabo.Core;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Linq;
    using System.Data;

    internal class SQLReader
    {
        private const string STORED_QUERY_FOLDER = "SQL";

        Dictionary<WebSocket, List<SQLReaderCommand>> storedReaders;

        public SQLReader()
        {
            this.storedReaders = new Dictionary<WebSocket, List<SQLReaderCommand>>();
        }

        public void Start()
        {
            WebSocketReader.Instance.Received += this.OnReceived;
        }

        public void Stop()
        {
            WebSocketReader.Instance.Received -= this.OnReceived;
        }

        private void OnReceived(object sender, WebSocketReceivedEventArgs e)
        {
            try
            {
                // Syntax: see SQLReaderCommand or id:int for follow up reading
                // e.g. see SQLReaderCommand as JSON serialized
                // e.g. 42

                // Library:
                // SQLReader.ExecuteQuery(query, Dictonary<string, string> wildcards...) : JsonObject[maxRows]
                // SQLReader.ExecuteStoredQuery(path, Dictonary<string, string> wildcards...) : JsonObject[maxRows]
                // SQLReader.ExecuteQuery(query, int maxRows, Dictonary<string, string> wildcards...) : QueryReader
                // SQLReader.ExecuteStoredQuery(path, int maxRows, Dictonary<string, string> wildcards...) : QueryReader
                // myQueryReader.GetNextResults() : JsonObject[maxRows] oder null wenn nichts mehr verfügbar

                int id;
                if (int.TryParse(e.Message, out id))
                {
                    // GetNextResults
                    // Continue reading already opened reader

                    SQLReaderCommand commandFound = null;
                    if (this.storedReaders.ContainsKey(e.WebSocket))
                    {
                        foreach (SQLReaderCommand command in this.storedReaders[e.WebSocket])
                        {
                            if (command.Id == id)
                            {
                                commandFound = command;
                                break;
                            }
                        }
                    }

                    if (commandFound != null)
                    {
                        Logging.Debug($"SQLReader continue reading request with id: {id} ({commandFound.TransferredRowCount} / {commandFound.BufferedData.Count})");

                        WebSocketHandler.SendTo(e.WebSocket, this.ReadRows(commandFound));

                        if (commandFound.TransferredRowCount >= commandFound.BufferedData.Count)
                        {
                            commandFound.TransferredRowCount = 0;
                            commandFound.BufferedData.Clear();
                        }
                    }
                    else
                    {
                        Logging.Error("SQLReader could not find id to continue reading: " + id);
                    }
                }
                else
                {
                    SQLReaderCommand command;
                    try
                    {
                        using (StringReader stream = new StringReader(e.Message))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            command = (SQLReaderCommand)serializer.Deserialize(stream, typeof(SQLReaderCommand));
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Error("SQLReader could not parse command: " + e.Message, ex);
                        return;
                    }

                    Logging.Debug("SQLReader request: " + command);

                    string query;

                    if (command.IsStoredQuery)
                    {
                        string absolutePath = Path.Combine(Program.Config.RootDirectory, SQLReader.STORED_QUERY_FOLDER, command.Query.Replace("/", "\\").TrimStart('\\'));
                        absolutePath = Uri.UnescapeDataString(absolutePath);
                        if (File.Exists(absolutePath) == false)
                        {
                            Logging.Error($"Query-File not found: {command.Query}");
                            return;
                        }

                        query = File.ReadAllText(absolutePath);
                    }
                    else
                    {
                        query = command.Query;
                    }

                    if (command.Wildcards != null)
                    {
                        foreach (KeyValuePair<string, string> wildcard in command.Wildcards)
                        {
                            query = query.Replace(wildcard.Key, wildcard.Value);
                        }
                    }

                    Database.Instance.ExecuteReader(query, (DatabaseReaderCallback data) =>
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        StringWriter stringWriter = new StringWriter(stringBuilder);
                        using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
                        {
                            jsonWriter.WriteStartObject();

                            int fields = data.Records.FieldCount;

                            for (int i = 0; i < fields; i++)
                            {
                                jsonWriter.WritePropertyName(data.Records.GetName(i));
                                jsonWriter.WriteValue(data.Records[i]);
                            }

                            jsonWriter.WriteEndObject();

                            command.BufferedData.Add(stringWriter.ToString());
                        }
                    });

                    if (command.MaxRows == null)
                    {
                        WebSocketHandler.SendTo(e.WebSocket, command.Id + ":" + this.ReadRows(command));
                    }
                    else
                    {
                        if (this.storedReaders.ContainsKey(e.WebSocket) == false)
                        {
                            this.storedReaders.Add(e.WebSocket, new List<SQLReaderCommand>());
                        }

                        foreach (SQLReaderCommand otherCommand in this.storedReaders[e.WebSocket])
                        {
                            if (otherCommand.Id == command.Id)
                            {
                                this.storedReaders[e.WebSocket].Remove(otherCommand);
                                break;
                            }
                        }

                        this.storedReaders[e.WebSocket].Add(command);
                    }
                }

                this.CleanUp();
            }
            catch (Exception ex)
            {
                Logging.Error($"Error during receiving SQLReader request: {e.Message}", ex);
            }
        }

        private string ReadRows(SQLReaderCommand command)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);
            using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.WriteStartArray();

                if (command.MaxRows == null)
                {
                    command.MaxRows = command.BufferedData.Count;
                }

                for (int i = command.TransferredRowCount; i < command.TransferredRowCount + command.MaxRows && i < command.BufferedData.Count; i++)
                {
                    jsonWriter.WriteRaw(command.BufferedData[i]);
                }

                command.TransferredRowCount += command.MaxRows.Value;

                jsonWriter.WriteEndArray();

                return stringWriter.ToString();
            }
        }

        private void CleanUp()
        {
            //foreach (List<SQLReaderCommand> commands in this.storedReaders.Values)
            for (int j = 0; j < this.storedReaders.Count; j++)
            {
                KeyValuePair<WebSocket, List<SQLReaderCommand>> commands = this.storedReaders.ElementAt(j);

                for (int i = 0; i < commands.Value.Count; i++)
                {
                    SQLReaderCommand command = commands.Value[i];

                    if ((DateTime.Now - command.Timestamp) > SQLReaderCommand.COMMAND_TIMEOUT)
                    {
                        commands.Value.Remove(command);
                        i--;
                    }
                }

                if (commands.Value.Count == 0)
                {
                    this.storedReaders.Remove(commands.Key);
                    j--;
                }
            }
        }
    }
}
