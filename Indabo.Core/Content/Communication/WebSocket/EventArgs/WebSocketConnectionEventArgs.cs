namespace Indabo.Core
{
    using System;
    using System.Net;

    internal class WebSocketConnectionEventArgs : EventArgs
    {
        private HttpListenerContext context = null;
        private string url = null;

        private bool isHandled = false;

        public HttpListenerContext Context { get => this.context; }
        public string Url { get => this.url; set => this.url = value;  }
        public bool IsHandled { get => this.isHandled; set => this.isHandled = value; }

        public WebSocketConnectionEventArgs(HttpListenerContext context, string url)
        {
            this.context = context;
            this.url = url;
        }
    }
}
