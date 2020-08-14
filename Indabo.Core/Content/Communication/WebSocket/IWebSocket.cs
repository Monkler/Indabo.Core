using System;

namespace Indabo.Core
{
    public class IWebSocket
    {
        public event EventHandler<WebSocketConnectionEventArgs> Opened;
        public event EventHandler<WebSocketReceivedEventArgs> Received;
        public event EventHandler<WebSocketConnectionEventArgs> Closed;

        private string url;

        protected IWebSocket(string url) 
        {
            this.url = url;

            WebSocketHandler.Instance.Opened += this.OnOpened;
            WebSocketHandler.Instance.Received += this.OnReceived;
            WebSocketHandler.Instance.Closed += this.OnClosed;
        }

        private void OnClosed(object sender, WebSocketConnectionEventArgs e)
        {
            if (e.Url == this.url)
            {
                this.Closed?.Invoke(this, e);
            }
        }

        private void OnOpened(object sender, WebSocketConnectionEventArgs e)
        {
            if (e.Url == this.url)
            {
                e.IsHandled = true;

                this.Opened?.Invoke(this, e);
            }
        }

        private void OnReceived(object sender, WebSocketReceivedEventArgs e)
        {
            if (e.Url == this.url)
            {
                this.Received?.Invoke(this, e);
            }
        }

        public void Send(string message)
        {
            WebSocketHandler.Instance.SendToAll(this.url, message);
        }
    }
}
