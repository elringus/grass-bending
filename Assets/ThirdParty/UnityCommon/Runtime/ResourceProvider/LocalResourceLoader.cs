using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon
{
    public class LocalResourceLoader<TResource> : LoadResourceRunner<TResource> where TResource : UnityEngine.Object
    {
        public string RootPath { get; private set; }

        private Action<string> logAction;
        private IRawConverter<TResource> converter;
        private byte[] rawData;

        public LocalResourceLoader (string rootPath, Resource<TResource> resource,
            IRawConverter<TResource> converter, Action<string> logAction)
        {
            RootPath = rootPath;
            Resource = resource;
            this.logAction = logAction;
            this.converter = converter;
        }

        public override async Task Run ()
        {
            await base.Run();

            var startTime = Time.time;

            var filePath = string.IsNullOrEmpty(RootPath) ? Resource.Path : string.Concat(RootPath, '/', Resource.Path);
            filePath = string.Concat(Application.dataPath, "/", filePath);

            foreach (var representation in converter.Representations)
            {
                var fullPath = string.Concat(filePath, representation.Extension);
                if (!File.Exists(fullPath)) continue;

                rawData = await IOUtils.ReadFileAsync(fullPath);
                break;
            }

            if (rawData == null)
            {
                var usedExtensions = string.Join("/", converter.Representations.Select(r => r.Extension));
                Debug.LogError($"Failed to load `{filePath}({usedExtensions})` resource using local file system: File not found.");
            }
            else
            {
                Resource.Object = await converter.ConvertAsync(rawData);
                logAction?.Invoke($"Resource `{Resource.Path}` loaded {StringUtils.FormatFileSize(rawData.Length)} over {Time.time - startTime:0.###} seconds.");
            }

            HandleOnCompleted();
        }
    }
}
