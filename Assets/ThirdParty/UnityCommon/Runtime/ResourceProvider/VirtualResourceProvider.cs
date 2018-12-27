using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityCommon
{
    /// <summary>
    /// A mock <see cref="IResourceProvider"/> implementation allowing to add resources at runtime.
    /// </summary>
    public class VirtualResourceProvider : IResourceProvider
    {
        public bool IsLoading => false;
        public float LoadProgress => 1;

        #pragma warning disable 0067
        public event Action<float> OnLoadProgress;
        public event Action<string> OnMessage;
        #pragma warning restore 0067

        protected Dictionary<string, UnityEngine.Object> Resources;
        protected HashSet<string> FolderPaths;

        public VirtualResourceProvider ()
        {
            Resources = new Dictionary<string, UnityEngine.Object>();
            FolderPaths = new HashSet<string>();
        }

        public bool SupportsType<T> () where T : UnityEngine.Object => true;

        public void AddResource (string path, UnityEngine.Object obj)
        {
            Resources[path] = new Resource(path, obj);
        }

        public void RemoveResource (string path)
        {
            Resources.Remove(path);
        }

        public void AddFolder (string folderPath)
        {
            FolderPaths.Add(folderPath);
        }

        public void RemoveFolder (string path)
        {
            FolderPaths.Remove(path);
        }

        public Resource<T> LoadResource<T> (string path) where T : UnityEngine.Object
        {
            Resources.TryGetValue(path, out var obj);
            return new Resource<T>(path, obj as T);
        }

        public Task<Resource<T>> LoadResourceAsync<T> (string path) where T : UnityEngine.Object
        {
            var resource = LoadResource<T>(path);
            return Task.FromResult(resource);
        }

        public IEnumerable<Resource<T>> LoadResources<T> (string path) where T : UnityEngine.Object
        {
            return Resources.Where(kv => kv.Value is T).Select(kv => kv.Key).LocateResourcePathsAtFolder(path).Select(p => LoadResource<T>(p));
        }

        public Task<IEnumerable<Resource<T>>> LoadResourcesAsync<T> (string path) where T : UnityEngine.Object
        {
            var resoucres = LoadResources<T>(path);
            return Task.FromResult(resoucres);
        }

        public IEnumerable<Folder> LocateFolders (string path)
        {
            return FolderPaths.LocateFolderPathsAtFolder(path).Select(p => new Folder(p));
        }

        public Task<IEnumerable<Folder>> LocateFoldersAsync (string path)
        {
            var folders = LocateFolders(path);
            return Task.FromResult(folders);
        }

        public IEnumerable<Resource<T>> LocateResources<T> (string path) where T : UnityEngine.Object
        {
            return Resources.Where(kv => kv.Value is T).Select(kv => kv.Key).LocateResourcePathsAtFolder(path).Select(p => new Resource<T>(p));
        }

        public Task<IEnumerable<Resource<T>>> LocateResourcesAsync<T> (string path) where T : UnityEngine.Object
        {
            var resources = LocateResources<T>(path);
            return Task.FromResult(resources);
        }

        public bool ResourceExists<T> (string path) where T : UnityEngine.Object
        {
            return Resources.ContainsKey(path) && Resources[path] is T;
        }

        public Task<bool> ResourceExistsAsync<T> (string path) where T : UnityEngine.Object
        {
            var result = ResourceExists<T>(path);
            return Task.FromResult(result);
        }

        public bool ResourceLoaded (string path)
        {
            return ResourceExists<UnityEngine.Object>(path);
        }

        public bool ResourceLoading (string path)
        {
            return false;
        }

        public void UnloadResource (string path)
        {
            Resources.Remove(path);
        }

        public Task UnloadResourceAsync (string path)
        {
            UnloadResource(path);
            return Task.CompletedTask;
        }

        public void UnloadResources ()
        {
            Resources.Clear();
        }

        public Task UnloadResourcesAsync ()
        {
            UnloadResources();
            return Task.CompletedTask;
        }
    }
}
