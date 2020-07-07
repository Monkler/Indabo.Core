namespace Indabo.Host
{
    using System.Collections.Generic;
    using System.IO;

    internal static class DynamicHtmlLoader
    {
        public const string PANEL_FOLDER = "./Panel";

        public static List<string> GetFileNamesFromFolder(string rootFolder)
        {
            List<string> fileNames = new List<string>();

            if (Directory.Exists(rootFolder))
            {
                string rootFolderAbsolutePath = (new FileInfo(rootFolder)).FullName + "\\";

                foreach (string file in Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories))
                {
                    if (file.EndsWith(".html"))
                    {
                        string absoulteFilePath = new FileInfo(file).FullName.Replace(".html", string.Empty);

                        fileNames.Add(absoulteFilePath.Replace(rootFolderAbsolutePath, string.Empty).Replace("\\", "/"));
                    }
                }
            }

            return fileNames;
        }
    }
}
