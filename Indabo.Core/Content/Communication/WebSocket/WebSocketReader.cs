namespace Indabo.Core
{
    public class WebSocketReader : IWebSocket
    {
        private static WebSocketReader instance;

        private WebSocketReader(string url = "/Reader") : base(url) { }

        public static IWebSocket Instance
        {
            get
            {
                if (WebSocketReader.instance == null)
                {
                    WebSocketReader.instance = new WebSocketReader();
                }

                return WebSocketReader.instance;
            }
        }
    }
}
