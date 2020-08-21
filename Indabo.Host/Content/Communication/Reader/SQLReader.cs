namespace Indabo.Host
{
    using System;
    using System.Data.Common;
    using System.IO;
    using System.Text;

    using Newtonsoft.Json;

    using Indabo.Core;

    internal class SQLReader
    {
        public SQLReader() {}

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
                // Folder /SQL durchsuchen ob vorhanden und die wildcards auflösen
                // Dann einfach Command ausführen von DBManager und response zurückgeben
                // Wildcards !?!
                // Wie weiß sender welcher respnse zu welchen request gehört 

                // evtl wirklich als json object
                // string id - um zu wissen welcher response zu welchen req gehört
                // bool isStoredQuery = true           bool isFilePath / isInlineQuery / isSideQuery / isRemoteQuery / isStoredQuery
                // string query oder string query-file-name / query-file-path
                // Dictionary<sring, string> parameters / wildcards // evtl auch nur list - noch mehr users porblem wenn er key zweimal macht // mache dann einfach query.Replace(dictonary.key, dicontary.value) was user daraus macht sein problem
                // int? maxRows = null // wenn nicht null dann werden maximal soviel rows zurückgegeben ... dannach frägt client wieder mit id and und bekommt wieder maxRows geliefert

                // in library dann // id wird intern abgehandelt einfach ein fortlaufende nummer (muss nich synchronisiert werden mit anderen clients)
                // SQLReader.ExecuteRemoteQuery(query, Dictonary<string, string> wildcards...) : JsonObject[maxRows]
                // SQLReader.ExecuteStoredQuery(path, Dictonary<string, string> wildcards...) : JsonObject[maxRows]
                // wie kann man hier weiter lesen... wenn man nicht alle auf einmal lesen will
                // SQLReader.ExecuteRemoteQuery(query, int maxRows, Dictonary<string, string> wildcards...) : QueryReader
                // SQLReader.ExecuteStoredQuery(path, int maxRows, Dictonary<string, string> wildcards...) : QueryReader
                // -> QueryReader.GetNextResults() : JsonObject[maxRows] oder null wenn nichts mehr verfügbar

                // wenn client mit selber id oder unbekannter id oder schon fertig gelesener id anfrägt einfach "null" zurückliefern

                Logging.Debug("SQLReader request: " + e.Message);

                string query;

                if (e.Message.StartsWith(":"))
                {
                    query = e.Message.Substring(1);
                }
                else
                {
                    string absolutePath = Path.Combine(Config.ROOT_DIRECTORY, e.Message.Replace("/", "\\"));
                    absolutePath = Uri.UnescapeDataString(absolutePath);
                    if (File.Exists(absolutePath) == false)
                    {
                        Logging.Error($"Query-File not found: {e.Message}");
                        return;
                    }

                    query = File.ReadAllText(absolutePath);
                }

                DbCommand command = Database.Instance.ExecuteCommand(query);
                DbDataReader reader = command.ExecuteReader();

                StringBuilder stringBuilder = new StringBuilder();
                StringWriter stringWriter = new StringWriter(stringBuilder);

                using (JsonWriter jsonWriter = new JsonTextWriter(stringWriter))
                {
                    jsonWriter.WriteStartArray();

                    while (reader.Read())
                    {
                        jsonWriter.WriteStartObject();

                        int fields = reader.FieldCount;

                        for (int i = 0; i < fields; i++)
                        {
                            jsonWriter.WritePropertyName(reader.GetName(i));
                            jsonWriter.WriteValue(reader[i]);
                        }

                        jsonWriter.WriteEndObject();
                    }

                    jsonWriter.WriteEndArray();
                }

                WebSocketHandler.SendTo(e.WebSocket, stringWriter.ToString());
            }
            catch (Exception ex)
            {
                Logging.Error($"Error during receiving SQLReader request: {e.Message}", ex);
            }
        }
    }
}
