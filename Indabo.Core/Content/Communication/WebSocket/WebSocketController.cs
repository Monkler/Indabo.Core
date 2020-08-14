namespace Indabo.Core
{
    public class WebSocketController : IWebSocket
    {
        private WebSocketController(string url = "/Controller") : base(url) { }

        public static IWebSocket Instance
        {
            get
            {
                if (IWebSocket.instance == null)
                {
                    IWebSocket.instance = new WebSocketController();
                }

                return IWebSocket.instance;
            }
        }
    }
}
