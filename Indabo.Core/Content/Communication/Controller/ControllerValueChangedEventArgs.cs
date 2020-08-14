namespace Indabo.Core
{
    public class ControllerValueChangedEventArgs
    {
        private string key;
        private string value;

        private bool isHandled;

        public string Key { get => this.key; }

        public string Value { get => this.value; }

        public bool IsHandled { get => this.isHandled; set => this.isHandled = value; }

        public ControllerValueChangedEventArgs(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
