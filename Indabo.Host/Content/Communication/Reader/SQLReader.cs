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

    internal class SQLReader
    {
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
                    foreach (SQLReaderCommand command in this.storedReaders[e.WebSocket])
                    {
                        if (command.Id == id)
                        {
                            commandFound = command;
                            break;
                        }
                    }

                    if (commandFound != null) 
                    {
                        Logging.Debug("SQLReader continue reading request with id: " + id);

                        WebSocketHandler.SendTo(e.WebSocket, this.ReadRows(commandFound.DataReader, commandFound.MaxRows));
                    }
                    else 
                    {
                        Logging.Error("SQLReader could not find id to continue reading: " + id);
                    }
                    
                }
                else
                {
                    SQLReaderCommand command = JsonConvert.DeserializeObject<SQLReaderCommand>(e.Message);                    

                    Logging.Debug("SQLReader request: " + command);

                    string query;

                    if (command.IsStoredQuery)
                    {
                        string absolutePath = Path.Combine(Config.ROOT_DIRECTORY, command.Query.Replace("/", "\\"));
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

                    foreach (KeyValuePair<string, string> wildcard in command.Wildcards)
                    {
                        query.Replace(wildcard.Key, wildcard.Value);
                    }

                    using (DbCommand dbCommand = Database.Instance.ExecuteCommand(query))
                    {
                        DbDataReader reader = dbCommand.ExecuteReader();
                        command.DataReader = reader;

                        if (command.MaxRows == null)
                        {
                            WebSocketHandler.SendTo(e.WebSocket, this.ReadRows(reader));
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
                                    otherCommand.DataReader.Close();
                                    this.storedReaders[e.WebSocket].Remove(otherCommand);
                                    break;
                                }
                            }

                            this.storedReaders[e.WebSocket].Add(command);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"Error during receiving SQLReader request: {e.Message}", ex);
            }
        }

        private string ReadRows(DbDataReader reader, int? maxRows = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(stringBuilder);

            bool isFinishedOrHasErrors = true;

            try
            {
                using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.WriteStartArray();

                    int lines = 0;
                    while ((maxRows == null || lines < maxRows) && reader.Read())
                    {
                        jsonWriter.WriteStartObject();

                        int fields = reader.FieldCount;

                        for (int i = 0; i < fields; i++)
                        {
                            jsonWriter.WritePropertyName(reader.GetName(i));
                            jsonWriter.WriteValue(reader[i]);
                        }

                        jsonWriter.WriteEndObject();

                        lines++;
                    }

                    if (lines >= maxRows)
                    {
                        isFinishedOrHasErrors = false;
                    }

                    jsonWriter.WriteEndArray();
                }
            }
            finally
            {
                if (isFinishedOrHasErrors)
                {
                    foreach (List<SQLReaderCommand> commands in this.storedReaders.Values)
                    {
                        //foreach (SQLReaderCommand command in commands)
                        for (int i = 0; i < commands.Count; i++)
                        {
                            SQLReaderCommand command = commands[i];

                            // Cleanup old readerss
                            if ((DateTime.Now - command.Timestamp) > SQLReaderCommand.COMMAND_TIMEOUT)
                            {
                                command.DataReader.Close();
                                commands.Remove(command);
                                i--;
                            }

                            if (command.DataReader == reader)
                            {
                                command.DataReader.Close();
                                commands.Remove(command);
                                i--;
                            }
                        }
                    }
                }
            }

            return stringWriter.ToString();
        }
    }
}
