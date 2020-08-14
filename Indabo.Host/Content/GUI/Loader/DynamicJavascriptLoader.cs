using System.IO;

namespace Indabo.Host
{
    internal static class DynamicJavascriptLoader
    {
        public const string WIDGET_FOLDER = "Widget";
        public const string LIBRARY_FOLDER = "Library";

        public static string LoadFromFolderAsHtmlTags(string rootFolder, string subFolder)
        {
            string script = string.Empty;

            string absolutePath = Path.Combine(rootFolder, subFolder);

            if (Directory.Exists(absolutePath))
            {
                string rootFolderAbsolutePath = (new FileInfo(absolutePath)).FullName + "\\";

                foreach (string file in Directory.GetFiles(absolutePath, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".js"))
                    {
                        string absoulteFilePath = new FileInfo(file).FullName.Replace(".html", string.Empty);

                        script += "<script src=\"/" + subFolder + "/" + absoulteFilePath.Replace(rootFolderAbsolutePath, string.Empty).Replace("\\", "/")  + "\" type=\"application/javascript\" charset=\"utf-8\">";
                        //script += File.ReadAllText(file);
                        script += "</script>";
                    }
                }                
            }

            return script;
        }
    }
}
