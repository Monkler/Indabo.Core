using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace Indabo.Core
{
    public class Notifier
    {
        private string notifierName;

        private Dictionary<string, List<WebSocket>> subscriptions;
        private Dictionary<string, string> retainBuffer;

        public Notifier(string notifierName)
        {
            if (notifierName.Contains("="))
            {
                throw new Exception("Notifier name should not contain '='");
            }

            this.subscriptions = new Dictionary<string, List<WebSocket>>();
            this.retainBuffer = new Dictionary<string, string>();
            this.notifierName = notifierName;

            WebSocketNotifier.Instance.Received += this.OnReceived;
            WebSocketNotifier.Instance.Closed += this.OnClosed;
        }

        private void OnClosed(object sender, WebSocketConnectionEventArgs e)
        {
            foreach (List<WebSocket> webSockets in this.subscriptions.Values)
            {
                foreach (WebSocket webSocket in webSockets)
                {
                    if (e.WebSocket == webSocket)
                    {
                        webSockets.Remove(e.WebSocket);
                        break;
                    }
                }
            }
        }

        private void OnReceived(object sender, WebSocketReceivedEventArgs e)
        {
            // Syntax: 
            // Subscribe: !<NotiferName>/Key1/Key2
            // Unsubscribe: ?<NotiferName>/Key1/Key2
            // e.g. !MyNotifier/MyTopic/MySubTopic

            try
            {
                if (e.Message.StartsWith("!" + this.notifierName + "/"))
                {
                    string key = e.Message.Substring(this.notifierName.Length + 2);

                    if (this.subscriptions.ContainsKey(key) == false)
                    {
                        this.subscriptions.Add(key, new List<WebSocket>());
                    }

                    if (this.subscriptions[key].Contains(e.WebSocket) == false)
                    {
                        this.subscriptions[key].Add(e.WebSocket);

                        Logging.Info("Subscription added: " + e.Message);

                        if (this.retainBuffer.ContainsKey(key))
                        {
                            this.Notify(key, this.retainBuffer[key], true, e.WebSocket);
                        }
                    }
                }
                else if (e.Message.StartsWith("?" + this.notifierName + "/"))
                {
                    string key = e.Message.Substring(this.notifierName.Length + 2);

                    if (this.subscriptions.ContainsKey(key))
                    {
                        if (this.subscriptions[key].Contains(e.WebSocket))
                        {
                            this.subscriptions[key].Remove(e.WebSocket);

                            Logging.Info("Subscription removed: " + e.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"Error during receiving notification request: {e.Message}", ex);
            }
        }

        public void Notify(string key, string value, bool retain = false, WebSocket sendToWebSocket = null)
        {
            try
            {
                if (this.subscriptions.ContainsKey(key))
                {
                    string message = this.notifierName + "/" + key + "=" + value;

                    if (sendToWebSocket == null)
                    {
                        foreach (WebSocket webSocket in this.subscriptions[key])
                        {                            
                            WebSocketHandler.SendTo(webSocket, message);
                        }
                    }
                    else
                    {
                        WebSocketHandler.SendTo(sendToWebSocket, message);
                    }

                    Logging.Debug("Notify: " + message);
                }

                if (retain == true)
                {
                    if (this.retainBuffer.ContainsKey(key) == false)
                    {
                        this.retainBuffer.Add(key, value);
                    }
                }

                if (this.retainBuffer.ContainsKey(key))
                {
                    this.retainBuffer[key] = value;
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"Could not send notification! Key: {key}, Value: {value}", ex);
            }
        }

        public void Notify(string key, int value, bool retain = false)
        {
            this.Notify(key, value.ToString(), retain);
        }

        public void Notify(string key, double value, bool retain = false)
        {
            this.Notify(key, value.ToString(), retain);
        }
    }
}
