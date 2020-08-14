namespace Indabo.Core
{
    public class WebSocketNotifier : IWebSocket
    {
        private static WebSocketNotifier instance;

        private WebSocketNotifier(string url = "/Notifier") : base(url) {}

        public static IWebSocket Instance
        {
            get
            {
                if (WebSocketNotifier.instance == null)
                {
                    WebSocketNotifier.instance = new WebSocketNotifier();
                }

                return WebSocketNotifier.instance;
            }
        }
    }
}
