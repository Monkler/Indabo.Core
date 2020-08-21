namespace Indabo.Core
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;

    public class Controller
    {
        public event EventHandler<ControllerValueChangedEventArgs> ValueChanged;

        private string controllerName;

        public Controller(string controllerName)
        {
            if (controllerName.Contains("="))
            {
                throw new Exception("Controller name should not contain '='");
            }

            this.controllerName = controllerName;

            WebSocketController.Instance.Received += this.OnReceived;
        }

        private void OnReceived(object sender, WebSocketReceivedEventArgs e)
        {
            // Syntax: <ControllerName>/Key1/Key2=Value
            // e.g. MyController/MyTopic/MySubTopic=42

            // Library:
            // let isHandledCallback = (key, value) => {}
            // Controller.send("MyNotifier/Key1/Key2", value)
            // Controller.send("MyNotifier/Key1/Key2", value, isHandledCallback)

            try
            {
                string key = e.Message.Split('=')[0].Trim(' ');

                if (key.StartsWith(this.controllerName + "/"))
                {
                    string[] valueArray = e.Message.Split('=');
                    string value = string.Empty;
                    for (int i = 1; i < valueArray.Length; i++)
                    {
                        value += valueArray[i];
                    }

                    Logging.Debug("Control received: " + e.Message);

                    ControllerValueChangedEventArgs eventArgs = new ControllerValueChangedEventArgs(key.Substring(this.controllerName.Length + 1), value);

                    try
                    {
                        this.ValueChanged?.Invoke(this, eventArgs);
                    }
                    catch (Exception ex)
                    {
                        Logging.Error($"Error while executing control request: Key: {key}, Value {value}");
                    }

                    if (eventArgs.IsHandled == true)
                    {
                        WebSocketController.Instance.Send(e.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Error($"Error during receiving control request: {e.Message}", ex);
            }
        }
    }
}
