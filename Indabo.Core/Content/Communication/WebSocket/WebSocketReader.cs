namespace Indabo.Core
{
    public class WebSocketReader : IWebSocket
    {
        private WebSocketReader(string url = "/Reader") : base(url) { }

        public static IWebSocket Instance
        {
            get
            {
                if (IWebSocket.instance == null)
                {
                    IWebSocket.instance = new WebSocketReader();
                }

                return IWebSocket.instance;
            }
        }
    }
}
