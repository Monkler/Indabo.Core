namespace Indabo.Host
{
    using System.Collections.Generic;
    using System.IO;

    internal static class DynamicHtmlLoader
    {
        public const string PANEL_FOLDER = "Panel";

        public static List<string> GetFileNamesFromFolder(string rootFolder, string subFolder)
        {
            List<string> fileNames = new List<string>();

            string absolutePath = Path.Combine(rootFolder, subFolder);

            if (Directory.Exists(absolutePath))
            {
                string rootFolderAbsolutePath = (new FileInfo(absolutePath)).FullName + "\\";

                foreach (string file in Directory.GetFiles(absolutePath, "*.*", SearchOption.AllDirectories))
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
