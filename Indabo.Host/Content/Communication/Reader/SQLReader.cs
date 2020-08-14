namespace Indabo.Host
{
    using Indabo.Core;
    using System;

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
                // Wie weiß sender welcher respnse zu welchen request gehört

                Logging.Debug("SQLReader request: " + e.Message);

                string response = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis abcdefghijklmnjklasdfasdf";

                WebSocketHandler.SendTo(e.WebSocket, response);
            }
            catch (Exception ex)
            {
                Logging.Error($"Error during receiving SQLReader request: {e.Message}", ex);
            }
        }
    }
}
