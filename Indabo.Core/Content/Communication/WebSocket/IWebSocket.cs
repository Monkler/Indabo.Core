using System;

namespace Indabo.Core
{
    public class IWebSocket
    {
        protected static IWebSocket instance;

        public event EventHandler<WebSocketReceivedEventArgs> Received;

        private string url;

        protected IWebSocket(string url) 
        {
            this.url = url;

            WebSocketHandler.Instance.Opened += this.OnInstanceOpened;
            WebSocketHandler.Instance.Received += this.OnInstanceReceived;
        }

        private void OnInstanceOpened(object sender, WebSocketConnectionEventArgs e)
        {
            if (e.Url == this.url)
            {
                e.IsHandled = true;
            }
        }

        private void OnInstanceReceived(object sender, WebSocketReceivedEventArgs e)
        {
            this.Received?.Invoke(this, e);
        }

        public void Send(string message)
        {
            WebSocketHandler.Instance.SendToAll(this.url, message);
        }
    }
}
