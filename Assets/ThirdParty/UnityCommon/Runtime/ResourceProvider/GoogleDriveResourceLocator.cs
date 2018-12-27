#if UNITY_GOOGLE_DRIVE_AVAILABLE

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityGoogleDrive;

namespace UnityCommon
{
    public class GoogleDriveResourceLocator<TResource> : LocateResourcesRunner<TResource> where TResource : UnityEngine.Object
    {
        public string RootPath { get; private set; }
        public string ResourcesPath { get; private set; }

        private IRawConverter<TResource> converter;

        public GoogleDriveResourceLocator (string rootPath, string resourcesPath, IRawConverter<TResource> converter)
        {
            RootPath = rootPath;
            ResourcesPath = resourcesPath;
            this.converter = converter;
        }

        public override async Task Run ()
        {
            await base.Run();

            LocatedResources = new List<Resource<TResource>>();

            // 1. Find all the files by path.
            var fullpath = Path.Combine(RootPath, ResourcesPath) + "/";
            var files = await Helpers.FindFilesByPathAsync(fullpath, fields: new List<string> { "files(name, mimeType)" });

            // 2. Filter the results by represenations (MIME types).
            var results = new Dictionary<RawDataRepresentation, List<UnityGoogleDrive.Data.File>>();
            foreach (var representation in converter.Representations)
                results.Add(representation, files.Where(f => f.MimeType == representation.MimeType).ToList());

            // 3. Create resources using located files.
            foreach (var result in results)
            {
                foreach (var file in result.Value)
                {
                    var fileName = string.IsNullOrEmpty(result.Key.Extension) ? file.Name : file.Name.GetBeforeLast(".");
                    var filePath = string.IsNullOrEmpty(ResourcesPath) ? fileName : string.Concat(ResourcesPath, '/', fileName);
                    var fileResource = new Resource<TResource>(filePath);
                    LocatedResources.Add(fileResource);
                }
            }

            HandleOnCompleted();
        }
    }
}

#endif
