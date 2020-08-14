namespace Indabo.Core
{
    using System;
    using System.Net;
    using System.Net.WebSockets;

    public class WebSocketConnectionEventArgs : EventArgs
    {
        private HttpListenerContext context = null;
        private WebSocket webSocket = null;
        private string url = null;

        private bool isHandled = false;

        public HttpListenerContext Context { get => this.context; }

        public WebSocket WebSocket { get => this.webSocket; set => this.webSocket = value; }

        public string Url { get => this.url; set => this.url = value;  }

        public bool IsHandled { get => this.isHandled; set => this.isHandled = value; }

        public WebSocketConnectionEventArgs(HttpListenerContext context, string url, WebSocket webSocket = null)
        {
            this.context = context;
            this.webSocket = webSocket;
            this.url = url;
        }
    }
}
