namespace Indabo.Core
{
    public class WebSocketNotifier : IWebSocket
    {
        private WebSocketNotifier(string url = "/Notifier") : base(url) {}

        public static IWebSocket Instance
        {
            get
            {
                if (IWebSocket.instance == null)
                {
                    IWebSocket.instance = new WebSocketNotifier();
                }

                return IWebSocket.instance;
            }
        }
    }
}
