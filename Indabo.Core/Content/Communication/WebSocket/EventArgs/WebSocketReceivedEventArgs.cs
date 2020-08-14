namespace Indabo.Core
{
    using System;
    using System.Net;
    using System.Net.WebSockets;

    public class WebSocketReceivedEventArgs : EventArgs
    {
        private WebSocket webSocket = null;
        private string url = null;

        private string message = null;

        public WebSocket WebSocket { get => this.webSocket; }

        public string Url { get => this.url; }

        public string Message { get => this.message; }

        public WebSocketReceivedEventArgs(WebSocket webSocket, string url, string message)
        {
            this.webSocket = webSocket;
            this.url = url;
            this.message = message;
        }
    }
}
