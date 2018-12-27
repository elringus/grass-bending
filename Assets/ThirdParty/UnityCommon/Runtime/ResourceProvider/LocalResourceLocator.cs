using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon
{
    public class LocalResourceLocator<TResource> : LocateResourcesRunner<TResource> where TResource : UnityEngine.Object
    {
        public string RootPath { get; private set; }
        public string ResourcesPath { get; private set; }

        private IRawConverter<TResource> converter;

        public LocalResourceLocator (string rootPath, string resourcesPath, IRawConverter<TResource> converter)
        {
            RootPath = rootPath;
            ResourcesPath = resourcesPath;
            this.converter = converter;
        }

        public override async Task Run ()
        {
            await base.Run();

            LocatedResources = LocateResources(RootPath, ResourcesPath, converter);

            HandleOnCompleted();
        }

        public static List<Resource<TResource>> LocateResources (string rootPath, string resourcesPath, IRawConverter<TResource> converter)
        {
            var locatedResources = new List<Resource<TResource>>();

            // 1. Resolving parent folder.
            var folderPath = Application.dataPath;
            if (!string.IsNullOrEmpty(rootPath) && !string.IsNullOrEmpty(resourcesPath))
                folderPath += string.Concat('/', rootPath, '/', resourcesPath);
            else if (string.IsNullOrEmpty(rootPath)) folderPath += string.Concat('/', resourcesPath);
            else folderPath += string.Concat('/', rootPath);
            var parendFolder = new DirectoryInfo(folderPath);
            if (!parendFolder.Exists) return locatedResources;

            // 2. Searching for the files in the folder.
            var results = new Dictionary<RawDataRepresentation, List<FileInfo>>();
            foreach (var representation in converter.Representations.DistinctBy(r => r.Extension))
            {
                var files = parendFolder.GetFiles(string.Concat("*", representation.Extension)).ToList();
                if (files != null && files.Count > 0) results.Add(representation, files);
            }

            // 3. Create resources using located files.
            foreach (var result in results)
            {
                foreach (var file in result.Value)
                {
                    var fileName = string.IsNullOrEmpty(result.Key.Extension) ? file.Name : file.Name.GetBeforeLast(".");
                    var filePath = string.IsNullOrEmpty(resourcesPath) ? fileName : string.Concat(resourcesPath, '/', fileName);
                    var fileResource = new Resource<TResource>(filePath);
                    locatedResources.Add(fileResource);
                }
            }

            return locatedResources;
        }
    }

}
