using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityCommon
{
    /// <summary>
    /// Implementation is able to load and unload <see cref="Resource"/> objects at runtime.
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// Event executed when load progress is changed.
        /// </summary>
        event Action<float> OnLoadProgress;
        /// <summary>
        /// Event executed when an information message is sent by the provider.
        /// </summary>
        event Action<string> OnMessage;

        /// <summary>
        /// Whether any resource loading operations are currently active.
        /// </summary>
        bool IsLoading { get; }
        /// <summary>
        /// Current resources loading progress, in 0.0 to 1.0 range.
        /// </summary>
        float LoadProgress { get; }

        /// <summary>
        /// Whether the provider can work with resource objects of the provided type.
        /// </summary>
        /// <typeparam name="T">Type of the resource object.</typeparam>
        bool SupportsType<T> () where T : UnityEngine.Object;
        /// <summary>
        /// Loads resource synchronously (blocking the calling thread).
        /// </summary>
        /// <typeparam name="T">Type of the resource to load.</typeparam>
        /// <param name="path">Path to the resource location.</param>
        Resource<T> LoadResource<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Loads resource asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of the resource to load.</typeparam>
        /// <param name="path">Path to the resource location.</param>
        Task<Resource<T>> LoadResourceAsync<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Loads all available resources at the provided path synchronously (blocking the calling thread).
        /// </summary>
        /// <typeparam name="T">Type of the resources to load.</typeparam>
        /// <param name="path">Path to the resources location.</param>
        IEnumerable<Resource<T>> LoadResources<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Loads all available resources at the provided path asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of the resources to load.</typeparam>
        /// <param name="path">Path to the resources location.</param>
        Task<IEnumerable<Resource<T>>> LoadResourcesAsync<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Locates all available resources at the provided path synchronously (blocking the calling thread).
        /// </summary>
        /// <typeparam name="T">Type of the resources to locate.</typeparam>
        /// <param name="path">Path to the resources location.</param>
        IEnumerable<Resource<T>> LocateResources<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Locates all available resources at the provided path asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of the resources to locate.</typeparam>
        /// <param name="path">Path to the resources location.</param>
        Task<IEnumerable<Resource<T>>> LocateResourcesAsync<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Locates all available folders at the provided path synchronously (blocking the calling thread).
        /// </summary>
        /// <param name="path">Path to the parent folder or empty string if none.</param>
        IEnumerable<Folder> LocateFolders (string path);
        /// <summary>
        /// Locates all available folders at the provided path asynchronously.
        /// </summary>
        /// <param name="path">Path to the parent folder or empty string if none.</param>
        Task<IEnumerable<Folder>> LocateFoldersAsync (string path);
        /// <summary>
        /// Checks whether resource with the provided type and path is available synchronously (blocking the calling thread).
        /// </summary>
        /// <typeparam name="T">Type of the resource to look for.</typeparam>
        /// <param name="path">Path to the resource location.</param>
        bool ResourceExists<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Checks whether resource with the provided type and path is available asynchronously.
        /// </summary>
        /// <typeparam name="T">Type of the resource to look for.</typeparam>
        /// <param name="path">Path to the resource location.</param>
        Task<bool> ResourceExistsAsync<T> (string path) where T : UnityEngine.Object;
        /// <summary>
        /// Unloads resource at the provided path synchronously (blocking the calling thread).
        /// </summary>
        /// <param name="path">Path to the resource location.</param>
        void UnloadResource (string path);
        /// <summary>
        /// Unloads resource at the provided path asynchronously.
        /// </summary>
        /// <param name="path">Path to the resource location.</param>
        Task UnloadResourceAsync (string path);
        /// <summary>
        /// Unloads all loaded resources synchronously (blocking the calling thread).
        /// </summary>
        void UnloadResources ();
        /// <summary>
        /// Unloads all loaded resources asynchronously.
        /// </summary>
        Task UnloadResourcesAsync ();
        /// <summary>
        /// Checks whether resource with the provided path is loaded.
        /// </summary>
        bool ResourceLoaded (string path);
        /// <summary>
        /// Checks whether resource with the provided path is currently being loaded.
        /// </summary>
        bool ResourceLoading (string path);
    }
}
