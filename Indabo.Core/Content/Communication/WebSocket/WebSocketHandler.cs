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
        internal event EventHandler<WebSocketConnectionEventArgs> Opened;
        internal event EventHandler<WebSocketReceivedEventArgs> Received;
        internal event EventHandler<WebSocketConnectionEventArgs> Closed;

        private static WebSocketHandler instance;

        Dictionary<string, List<WebSocket>> webSockets = new Dictionary<string, List<WebSocket>>();

        private WebSocketHandler() {}

        public async void HandleWebSocketRequestAsync(HttpListenerContext context)
        {
            WebSocket webSocket = null;
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

                webSocket = webSocketContext.WebSocket;
                if (this.webSockets.ContainsKey(webSocketConnectionEventArgs.Url) == false)
                {
                    this.webSockets.Add(webSocketConnectionEventArgs.Url, new List<WebSocket>());
                }
                this.webSockets[webSocketConnectionEventArgs.Url].Add(webSocket);

                string message = string.Empty;

                byte[] receiveBuffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    
                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    }
                    else if (receiveResult.MessageType != WebSocketMessageType.Text)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Only accept text frame", CancellationToken.None);
                    }
                    else if (receiveResult.EndOfMessage == false)
                    {
                        message += Encoding.UTF8.GetString(receiveBuffer);
                    }
                    else
                    {
                        this.Received?.Invoke(this, new WebSocketReceivedEventArgs(webSocket, webSocketConnectionEventArgs.Url, message));
                    }
                }

            }
            catch (Exception ex)
            {
                Logging.Error("Error while handling Websocket request!", ex);
            }
            finally {
                if (webSocket != null)
                {
                    this.Closed?.Invoke(this, webSocketConnectionEventArgs);
                    this.webSockets[webSocketConnectionEventArgs.Url].Remove(webSocket);
                    webSocket.Dispose();
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
                    webSocket.SendAsync(new ArraySegment<byte>(messageBuffer, 0, messageBuffer.Length), WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
            else
            {
                Logging.Warning($"Could not send message - Websocket does not exist: '{url}'");
            }
        }

        public static void SendTo(WebSocket webSocket, string message)
        {
            WebSocketHandler.SendTo(webSocket, Encoding.UTF8.GetBytes(message));
        }

        public static void SendTo(WebSocket webSocket, byte[] messageBuffer)
        {
            for (int i = 0; i < messageBuffer.Length; i += 1024)
            {
                int length = 1024;
                if (i + length > messageBuffer.Length)
                {
                    length = messageBuffer.Length - i;
                }

                webSocket.SendAsync(new ArraySegment<byte>(messageBuffer, i, length), WebSocketMessageType.Text, true, CancellationToken.None);
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
