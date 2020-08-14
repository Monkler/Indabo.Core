namespace Indabo.Core
{
    public class WebSocketController : IWebSocket
    {
        private static WebSocketController instance;

        private WebSocketController(string url = "/Controller") : base(url) { }

        public static IWebSocket Instance
        {
            get
            {
                if (WebSocketController.instance == null)
                {
                    WebSocketController.instance = new WebSocketController();
                }

                return WebSocketController.instance;
            }
        }
    }
}
