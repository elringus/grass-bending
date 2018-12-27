#if UNITY_GOOGLE_DRIVE_AVAILABLE

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityGoogleDrive;

namespace UnityCommon
{
    public class GoogleDriveFolderLocator : LocateFoldersRunner
    {
        public string RootPath { get; private set; }
        public string ResourcesPath { get; private set; }

        public GoogleDriveFolderLocator (string rootPath, string resourcesPath)
        {
            RootPath = rootPath;
            ResourcesPath = resourcesPath;
        }

        public override async Task Run ()
        {
            await base.Run();

            LocatedFolders = new List<Folder>();

            var fullpath = Path.Combine(RootPath, ResourcesPath) + "/";
            var gFolders = await Helpers.FindFilesByPathAsync(fullpath, fields: new List<string> { "files(name)" }, mime: "application/vnd.google-apps.folder");

            foreach (var gFolder in gFolders)
            {
                var folderPath = string.IsNullOrEmpty(ResourcesPath) ? gFolder.Name : string.Concat(ResourcesPath, '/', gFolder.Name);
                var folder = new Folder(folderPath);
                LocatedFolders.Add(folder);
            }

            HandleOnCompleted();
        }
    }
}

#endif
