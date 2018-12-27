using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityCommon
{
    public class RemoteResourceProvider : IResourceProvider
    {
        #pragma warning disable 67
        public event Action<float> OnLoadProgress;
        public event Action<string> OnMessage;
        #pragma warning restore 67

        public bool IsLoading { get { throw new NotImplementedException(); } }
        public float LoadProgress { get { throw new NotImplementedException(); } }

        public bool SupportsType<T> () where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public Resource<T> LoadResource<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public Task<Resource<T>> LoadResourceAsync<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Resource<T>> LoadResources<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Resource<T>>> LoadResourcesAsync<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Resource<T>> LocateResources<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Resource<T>>> LocateResourcesAsync<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Folder> LocateFolders (string path)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Folder>> LocateFoldersAsync (string path)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResourceExistsAsync<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public void UnloadResource (string path)
        {
            throw new NotImplementedException();
        }

        public void UnloadResources ()
        {
            throw new NotImplementedException();
        }

        public bool ResourceExists<T> (string path) where T : UnityEngine.Object
        {
            throw new NotImplementedException();
        }

        public Task UnloadResourceAsync (string path)
        {
            throw new NotImplementedException();
        }

        public Task UnloadResourcesAsync ()
        {
            throw new NotImplementedException();
        }

        public bool ResourceLoaded (string path)
        {
            throw new NotImplementedException();
        }

        public bool ResourceLoading (string path)
        {
            throw new NotImplementedException();
        }
    }
}
