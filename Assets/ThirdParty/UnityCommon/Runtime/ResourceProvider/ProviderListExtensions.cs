using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityCommon
{
    public static class ProviderListExtensions
    {
        /// <summary>
        /// Loads a resource at the provided path.
        /// When resources with equal paths are available in multiple providers, will load the one from the higher-priority provider.
        /// </summary>
        public static Resource<T> LoadResource<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            var resource = default(Resource<T>);
            if (providers.Count == 1)
                resource = providers[0].LoadResource<T>(path);
            else
            {
                foreach (var provider in providers)
                {
                    if (!provider.ResourceExists<T>(path)) continue;
                    resource = provider.LoadResource<T>(path);
                    break;
                }
            }
            return resource;
        }

        /// <summary>
        /// Loads a resource at the provided path.
        /// When resources with equal paths are available in multiple providers, will load the one from the higher-priority provider.
        /// </summary>
        public static async Task<Resource<T>> LoadResourceAsync<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            var resource = default(Resource<T>);
            if (providers.Count == 1)
                resource = await providers[0].LoadResourceAsync<T>(path);
            else
            {
                foreach (var provider in providers)
                {
                    if (!await provider.ResourceExistsAsync<T>(path)) continue;
                    resource = await provider.LoadResourceAsync<T>(path);
                    break;
                }
            }
            return resource;
        }

        /// <summary>
        /// Loads all the resources at the provided path from all the providers.
        /// When a resource is available in multiple providers, will only load the one from the higher-priority provider.
        /// </summary>
        public static IEnumerable<Resource<T>> LoadResources<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            var resources = new List<Resource<T>>();
            if (providers.Count == 1)
                resources.AddRange(providers[0].LoadResources<T>(path));
            else
            {
                foreach (var provider in providers)
                {
                    var locatedResources =  provider.LocateResources<T>(path);
                    foreach (var locatedResource in locatedResources)
                        if (!resources.Any(r => r.Path == locatedResource.Path))
                            resources.Add( provider.LoadResource<T>(locatedResource.Path));
                }
            }
            return resources;
        }

        /// <summary>
        /// Loads all the resources at the provided path from all the providers.
        /// When a resource is available in multiple providers, will only load the one from the higher-priority provider.
        /// </summary>
        public static async Task<IEnumerable<Resource<T>>> LoadResourcesAsync<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            var resources = new List<Resource<T>>();
            if (providers.Count == 1)
                resources.AddRange(await providers[0].LoadResourcesAsync<T>(path));
            else
            {
                foreach (var provider in providers)
                {
                    var locatedResources = await provider.LocateResourcesAsync<T>(path);
                    foreach (var locatedResource in locatedResources)
                        if (!resources.Any(r => r.Path == locatedResource.Path))
                            resources.Add(await provider.LoadResourceAsync<T>(locatedResource.Path));
                }
            }
            return resources;
        }

        /// <summary>
        /// Locates all the resources at the provided path from all the providers.
        /// When a resource is available in multiple providers, will only get the one from the higher-priority provider.
        /// </summary>
        public static IEnumerable<Resource<T>> LocateResources<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            var result = new List<Resource<T>>();
            foreach (var provider in providers)
            {
                var locatedResources =  provider.LocateResources<T>(path);
                foreach (var locatedResource in locatedResources)
                    if (!result.Any(r => r.Path == locatedResource.Path))
                        result.Add(locatedResource);
            }
            return result;
        }

        /// <summary>
        /// Locates all the resources at the provided path from all the providers.
        /// When a resource is available in multiple providers, will only get the one from the higher-priority provider.
        /// </summary>
        public static async Task<IEnumerable<Resource<T>>> LocateResourcesAsync<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            var result = new List<Resource<T>>();
            foreach (var provider in providers)
            {
                var locatedResources = await provider.LocateResourcesAsync<T>(path);
                foreach (var locatedResource in locatedResources)
                    if (!result.Any(r => r.Path == locatedResource.Path))
                        result.Add(locatedResource);
            }
            return result;
        }

        /// <summary>
        /// Locates all the folders at the provided path from all the providers.
        /// When a folder is available in multiple providers, will only get the one from the higher-priority provider.
        /// </summary>
        public static IEnumerable<Folder> LocateFolders (this List<IResourceProvider> providers, string path)
        {
            var result = new List<Folder>();
            foreach (var provider in providers)
            {
                var locatedFolders = provider.LocateFolders(path);
                foreach (var locatedFolder in locatedFolders)
                    if (!result.Any(r => r.Path == locatedFolder.Path))
                        result.Add(locatedFolder);
            }
            return result;
        }

        /// <summary>
        /// Locates all the folders at the provided path from all the providers.
        /// When a folder is available in multiple providers, will only get the one from the higher-priority provider.
        /// </summary>
        public static async Task<IEnumerable<Folder>> LocateFoldersAsync (this List<IResourceProvider> providers, string path)
        {
            var result = new List<Folder>();
            foreach (var provider in providers)
            {
                var locatedFolders = await provider.LocateFoldersAsync(path);
                foreach (var locatedFolder in locatedFolders)
                    if (!result.Any(r => r.Path == locatedFolder.Path))
                        result.Add(locatedFolder);
            }
            return result;
        }

        /// <summary>
        /// Checks whether a resource at the provided path exists in any of the providers.
        /// </summary>
        public static bool ResourceExists<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            foreach (var provider in providers)
                if (provider.ResourceExists<T>(path)) return true;
            return false;
        }

        /// <summary>
        /// Checks whether a resource at the provided path exists in any of the providers.
        /// </summary>
        public static async Task<bool> ResourceExistsAsync<T> (this List<IResourceProvider> providers, string path) where T : UnityEngine.Object
        {
            foreach (var provider in providers)
                if (await provider.ResourceExistsAsync<T>(path)) return true;
            return false;
        }

        /// <summary>
        /// Unloads resource at the provided path from all the providers in the list.
        /// </summary>
        /// <param name="providers">Providers list.</param>
        /// <param name="path">Path to the resource location.</param>
        public static void UnloadResource (this List<IResourceProvider> providers, string path)
        {
            foreach (var provider in providers)
                 provider.UnloadResource(path);
        }

        /// <summary>
        /// Unloads resource at the provided path from all the providers in the list.
        /// </summary>
        /// <param name="providers">Providers list.</param>
        /// <param name="path">Path to the resource location.</param>
        public static async Task UnloadResourceAsync (this List<IResourceProvider> providers, string path)
        {
            foreach (var provider in providers)
                await provider.UnloadResourceAsync(path);
        }

        /// <summary>
        /// Unloads all loaded resources from all the providers in the list.
        /// </summary>
        public static void UnloadResources (this List<IResourceProvider> providers)
        {
            foreach (var provider in providers)
                 provider.UnloadResources();
        }

        /// <summary>
        /// Unloads all loaded resources from all the providers in the list.
        /// </summary>
        public static async Task UnloadResourcesAsync (this List<IResourceProvider> providers)
        {
            foreach (var provider in providers)
                await provider.UnloadResourcesAsync();
        }

        /// <summary>
        /// Checks whether resource with the provided path is loaded by any of the providers in the list.
        /// </summary>
        public static bool ResourceLoaded (this List<IResourceProvider> providers, string path)
        {
            foreach (var provider in providers)
                if (provider.ResourceLoaded(path)) return true;
            return false;
        }

        /// <summary>
        /// Checks whether resource with the provided path is currently being loaded by any of the providers in the list.
        /// </summary>
        public static bool ResourceLoading (this List<IResourceProvider> providers, string path)
        {
            foreach (var provider in providers)
                if (provider.ResourceLoading(path)) return true;
            return false;
        }
    }
}
