namespace Indabo.Windows.Launcher
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    internal class LogTypeToImageConverter : IValueConverter
    {
        private const string COMPONENT_PATH = "/Indabo.Windows.Launcher;component/";
        private const string ICON_PATH = "Resources/Icon/";
        private const string ICON_ENDING = ".png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BitmapImage(new Uri(COMPONENT_PATH + ICON_PATH + value.ToString().ToLower() + ICON_ENDING, UriKind.Relative));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
