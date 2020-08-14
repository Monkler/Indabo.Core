namespace Indabo.Host
{
    using Indabo.Core;

    internal class SQLReader
    {
        private SQLReader instance;

        public SQLReader() {}

        public void Start()
        {
            WebSocketReader.Instance.Received += this.OnInstanceReceived;
        }

        public void Stop()
        {
            WebSocketReader.Instance.Received -= this.OnInstanceReceived;
        }

        private void OnInstanceReceived(object sender, WebSocketReceivedEventArgs e)
        {
            string response = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat, vel illum dolore eu feugiat nulla facilisis abcdefghijklmnjklasdfasdf";

            WebSocketHandler.SendTo(e.WebSocket, response);
        }
    }
}
