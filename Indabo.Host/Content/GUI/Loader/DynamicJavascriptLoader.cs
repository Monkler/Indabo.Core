using System.IO;

namespace Indabo.Host
{
    internal static class DynamicJavascriptLoader
    {
        public const string WIDGET_FOLDER = "Widget";
        public const string LIBRARY_FOLDER = "Library";

        public static string LoadFromFolderAsHtmlTags(string rootFolder)
        {
            string script = string.Empty;

            if (Directory.Exists(rootFolder))
            {
                foreach (string file in Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".js"))
                    {
                        script += "<script type=\"application/javascript\" charset=\"utf-8\">";
                        script += File.ReadAllText(file);
                        script += "</script>";
                    }
                }                
            }

            return script;
        }
    }
}
