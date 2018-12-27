using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityCommon
{
    /// <summary>
    /// Allows working with resources using a prioritized providers list.
    /// </summary>
    public abstract class ResourceLoader
    {
        public abstract bool IsLoadingAny { get; }
        public string PathPrefix { get; }

        protected List<IResourceProvider> Providers { get; }
        protected VirtualResourceProvider VirtualProvider { get; }

        public ResourceLoader (IList<IResourceProvider> providersList, string resourcePathPrefix = null)
        {
            Providers = new List<IResourceProvider>();
            VirtualProvider = new VirtualResourceProvider();
            Providers.Add(VirtualProvider);
            Providers.AddRange(providersList);
            PathPrefix = resourcePathPrefix;
        }

        /// <summary>
        /// Given a local path to the resource, builds full path using predefined <see cref="PathPrefix"/>.
        /// </summary>
        public virtual string BuildFullPath (string path)
        {
            if (!string.IsNullOrWhiteSpace(PathPrefix))
            {
                if (!string.IsNullOrWhiteSpace(path)) return $"{PathPrefix}/{path}";
                else return PathPrefix;
            }
            else return path;
        }

        public abstract void Preload (string path, bool isFullPath = false);
        public abstract Task PreloadAsync (string path, bool isFullPath = false);
        public abstract bool IsLoadedByProvider (string path, bool isFullPath = false);
        public abstract bool IsLoaded (string path, bool isFullPath = false);
        public abstract void Unload (string path, bool isFullPath = false);
        public abstract Task UnloadAsync (string path, bool isFullPath = false);
        public abstract void UnloadAll ();
        public abstract Task UnloadAllAsync ();
    }

    /// <summary>
    /// Allows working with resources of specific type using a prioritized providers list.
    /// </summary>
    public class ResourceLoader<TResource> : ResourceLoader where TResource : UnityEngine.Object
    {
        public override bool IsLoadingAny => loadCounter > 0;

        protected Dictionary<string, TResource> LoadedResources { get; }

        private int loadCounter;

        public ResourceLoader (IList<IResourceProvider> providersList, string resourcePathPrefix = null, IDictionary<string, TResource> preloadedResources = null)
            : base(providersList, resourcePathPrefix)
        {
            LoadedResources = new Dictionary<string, TResource>();

            if (preloadedResources != null)
                foreach (var kv in preloadedResources)
                    AddPreloadedResource(kv.Key, kv.Value);
        }

        public virtual void AddPreloadedResource (string path, TResource resourceObj, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            VirtualProvider.AddResource(path, resourceObj);
        }

        public virtual void RemovePreloadedResource (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            VirtualProvider.RemoveResource(path);
        }

        public virtual TResource Load (string path, bool isFullPath = false)
        {
            if (IsLoaded(path, isFullPath)) return GetLoaded(path, isFullPath);

            IncrementLoadCounter();
            if (!isFullPath) path = BuildFullPath(path);

            var resource = Providers.LoadResource<TResource>(path);
            AddLoadedResource(resource);

            DecrementLoadCounter();
            return resource != null && resource.IsValid ? resource.Object : null;
        }

        public virtual async Task<TResource> LoadAsync (string path, bool isFullPath = false)
        {
            if (IsLoaded(path, isFullPath)) return GetLoaded(path, isFullPath);

            IncrementLoadCounter();
            if (!isFullPath) path = BuildFullPath(path);

            var resource = await Providers.LoadResourceAsync<TResource>(path);
            AddLoadedResource(resource);

            DecrementLoadCounter();
            return resource != null && resource.IsValid ? resource.Object : null;
        }

        public virtual IEnumerable<Resource<TResource>> LoadAll (string path = null, bool isFullPath = false)
        {
            IncrementLoadCounter();
            if (!isFullPath) path = BuildFullPath(path);

            var resources = Providers.LoadResources<TResource>(path);
            AddLoadedResources(resources);

            DecrementLoadCounter();
            return resources;
        }

        public virtual async Task<IEnumerable<Resource<TResource>>> LoadAllAsync (string path = null, bool isFullPath = false)
        {
            IncrementLoadCounter();
            if (!isFullPath) path = BuildFullPath(path);

            var resources = await Providers.LoadResourcesAsync<TResource>(path);
            AddLoadedResources(resources);

            DecrementLoadCounter();
            return resources;
        }

        public virtual IEnumerable<Resource<TResource>> LocateResources (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            return Providers.LocateResources<TResource>(path);
        }

        public virtual async Task<IEnumerable<Resource<TResource>>> LocateResourcesAsync (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            return await Providers.LocateResourcesAsync<TResource>(path);
        }

        public virtual bool ResourceExists (string path, bool isFullPath = false)
        {
            if (IsLoaded(path, isFullPath) || IsLoadedByProvider(path, isFullPath)) return true;

            if (!isFullPath) path = BuildFullPath(path);
            return Providers.ResourceExists<TResource>(path);
        }

        public virtual async Task<bool> ResourceExistsAsync (string path, bool isFullPath = false)
        {
            if (IsLoaded(path, isFullPath) || IsLoadedByProvider(path, isFullPath)) return true;

            if (!isFullPath) path = BuildFullPath(path);
            return await Providers.ResourceExistsAsync<TResource>(path);
        }

        public override void Preload (string path, bool isFullPath = false)
        {
            Load(path, isFullPath);
        }

        public override async Task PreloadAsync (string path, bool isFullPath = false)
        {
            await LoadAsync(path, isFullPath);
        }

        public override void Unload (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);

            Providers.UnloadResource(path);

            RemoveLoadedResource(path);
        }

        public override async Task UnloadAsync (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);

            await Providers.UnloadResourceAsync(path);

            RemoveLoadedResource(path);
        }

        public override void UnloadAll ()
        {
            var resources = LoadedResources.Keys.ToArray();
            for (int i = 0; i < resources.Length; i++)
                Unload(resources[i], true);
        }

        public override async Task UnloadAllAsync ()
        {
            var resources = LoadedResources.Keys.ToArray();
            for (int i = 0; i < resources.Length; i++)
                await UnloadAsync(resources[i], true);
        }

        /// <summary>
        /// Whether a resource with the provided path is loaded by any of the available providers.
        /// </summary>
        public override bool IsLoadedByProvider (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            return Providers.ResourceLoaded(path);
        }

        /// <summary>
        /// Whether a resource with the provided path is already loaded 
        /// and can be instantly retrieved via <see cref="GetLoaded(string, bool)"/>.
        /// </summary>
        public override bool IsLoaded (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            return LoadedResources.ContainsKey(path);
        }

        /// <summary>
        /// Returns a loaded resource with the provided path.
        /// In case the resource is not loaded, will return null.
        /// </summary>
        public virtual TResource GetLoaded (string path, bool isFullPath = false)
        {
            if (!isFullPath) path = BuildFullPath(path);
            if (!IsLoaded(path, true)) return default;
            return LoadedResources[path];
        }

        protected virtual void AddLoadedResources (IEnumerable<Resource<TResource>> resources)
        {
            foreach (var resource in resources)
                if (resource != null && resource.IsValid)
                    AddLoadedResource(resource);
        }

        protected virtual void AddLoadedResource (Resource<TResource> resource)
        {
            if (resource != null && resource.IsValid)
                LoadedResources[resource.Path] = resource.Object;
        }

        protected virtual void RemoveLoadedResource (string resourcePath)
        {
            if (LoadedResources.ContainsKey(resourcePath))
                LoadedResources.Remove(resourcePath);
        }

        protected void IncrementLoadCounter () => loadCounter++;
        protected void DecrementLoadCounter () => loadCounter--;
    }
}
