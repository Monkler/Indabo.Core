namespace Indabo.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using Indabo.Core;

    public class WebSocketHandler
    {
        private const int TRANSMIT_BUFFER_SIZE = 1024;
        private const int RECEIVE_BUFFER_SIZE = 1024;

        internal event EventHandler<WebSocketConnectionEventArgs> Opened;
        internal event EventHandler<WebSocketReceivedEventArgs> Received;
        internal event EventHandler<WebSocketConnectionEventArgs> Closed;

        private static WebSocketHandler instance;

        Dictionary<string, List<WebSocket>> webSockets = new Dictionary<string, List<WebSocket>>();

        private WebSocketHandler() { }

        public async void HandleWebSocketRequestAsync(HttpListenerContext context)
        {
            WebSocketConnectionEventArgs webSocketConnectionEventArgs = new WebSocketConnectionEventArgs(context, context.Request.RawUrl);

            try
            {
                this.Opened?.Invoke(this, webSocketConnectionEventArgs);

                if (webSocketConnectionEventArgs.IsHandled == false)
                {
                    Logging.Warning($"WebSocket is not supported: '{context.Request.RawUrl}'!");

                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;

                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    byte[] buffer = Encoding.UTF8.GetBytes($"WebSocket is not supported: '{context.Request.RawUrl}'!");

                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();

                    return;
                }

                WebSocketContext webSocketContext = await context.AcceptWebSocketAsync(null);

                webSocketConnectionEventArgs.WebSocket = webSocketContext.WebSocket;
                if (this.webSockets.ContainsKey(webSocketConnectionEventArgs.Url) == false)
                {
                    this.webSockets.Add(webSocketConnectionEventArgs.Url, new List<WebSocket>());
                }
                this.webSockets[webSocketConnectionEventArgs.Url].Add(webSocketConnectionEventArgs.WebSocket);

                string message = string.Empty;

                byte[] receiveBuffer = new byte[RECEIVE_BUFFER_SIZE];
                int messageLength = 0;
                while (webSocketConnectionEventArgs.WebSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocketConnectionEventArgs.WebSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    messageLength += receiveResult.Count;

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocketConnectionEventArgs.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else if (receiveResult.MessageType != WebSocketMessageType.Text)
                    {
                        await webSocketConnectionEventArgs.WebSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Only accept text frame", CancellationToken.None);
                    }
                    else
                    {
                        message += Encoding.UTF8.GetString(receiveBuffer, 0, messageLength);

                        if (receiveResult.EndOfMessage)
                        {
                            this.Received?.Invoke(this, new WebSocketReceivedEventArgs(webSocketConnectionEventArgs.WebSocket, webSocketConnectionEventArgs.Url, message));
                            message = string.Empty;
                            messageLength = 0;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Logging.Error("Error while handling Websocket request!", ex);
            }
            finally
            {
                if (webSocketConnectionEventArgs.WebSocket != null)
                {
                    this.Closed?.Invoke(this, webSocketConnectionEventArgs);
                    this.webSockets[webSocketConnectionEventArgs.Url].Remove(webSocketConnectionEventArgs.WebSocket);
                    webSocketConnectionEventArgs.WebSocket.Dispose();
                }
            }
        }

        public void SendToAll(string url, string message)
        {
            if (this.webSockets.ContainsKey(url))
            {
                byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
                foreach (WebSocket webSocket in this.webSockets[url])
                {
                    WebSocketHandler.SendTo(webSocket, messageBuffer);
                }
            }
            else
            {
                Logging.Warning($"Could not send message - Websocket does not exist: '{url}'");
            }
        }

        public static void SendTo(WebSocket webSocket, string message, int? length = null)
        {
            WebSocketHandler.SendTo(webSocket, Encoding.UTF8.GetBytes(message));
        }

        public static void SendTo(WebSocket webSocket, byte[] messageBuffer, int? length = null)
        {
            try
            {
                if (length == null)
                {
                    length = messageBuffer.Length;
                }

                lock (webSocket)
                {
                    for (int i = 0; i < messageBuffer.Length; i += TRANSMIT_BUFFER_SIZE)
                    {
                        int currentLEngth = TRANSMIT_BUFFER_SIZE;
                        if (i + currentLEngth > length)
                        {
                            currentLEngth = messageBuffer.Length - i;
                        }

                        webSocket.SendAsync(new ArraySegment<byte>(messageBuffer, i, currentLEngth), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"Could not send message to webSocket!", ex);
            }
        }

        public static WebSocketHandler Instance
        {
            get
            {
                if (WebSocketHandler.instance == null)
                {
                    WebSocketHandler.instance = new WebSocketHandler();
                }

                return WebSocketHandler.instance;
            }
        }
    }
}
