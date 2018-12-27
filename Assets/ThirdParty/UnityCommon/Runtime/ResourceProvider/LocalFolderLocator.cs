using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon
{
    public class LocalFolderLocator : LocateFoldersRunner
    {
        public string RootPath { get; private set; }
        public string ResourcesPath { get; private set; }

        public LocalFolderLocator (string rootPath, string resourcesPath)
        {
            RootPath = rootPath;
            ResourcesPath = resourcesPath;
        }

        public override async Task Run ()
        {
            await base.Run();

            LocatedFolders = LocateFoldersAtPath(RootPath, ResourcesPath);

            HandleOnCompleted();
            return;
        }

        public static List<Folder> LocateFoldersAtPath (string rootPath, string resourcesPath)
        {
            var locatedFolders = new List<Folder>();

            var folderPath = Application.dataPath;
            if (!string.IsNullOrEmpty(rootPath) && !string.IsNullOrEmpty(resourcesPath))
                folderPath += string.Concat('/', rootPath, '/', resourcesPath);
            else if (string.IsNullOrEmpty(rootPath)) folderPath += string.Concat('/', resourcesPath);
            else folderPath += string.Concat('/', rootPath);
            var parendFolder = new DirectoryInfo(folderPath);
            if (!parendFolder.Exists) return locatedFolders;

            foreach (var dir in parendFolder.GetDirectories())
            {
                var path = dir.FullName.Replace("\\", "/").GetAfterFirst(rootPath + "/");
                var folder = new Folder(path);
                locatedFolders.Add(folder);
            }

            return locatedFolders;
        }
    }
}
