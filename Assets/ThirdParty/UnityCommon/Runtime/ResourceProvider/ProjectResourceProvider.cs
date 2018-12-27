using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Provides resources stored in the 'Resources' folders of the project.
    /// </summary>
    public class ProjectResourceProvider : ResourceProvider
    {
        public class TypeRedirector
        {
            public Type SourceType { get; private set; }
            public Type RedirectType { get; private set; }
            public IConverter RedirectToSourceConverter { get; private set; }

            public TypeRedirector (Type sourceType, Type redirectType, IConverter redirectToSourceConverter)
            {
                SourceType = sourceType;
                RedirectType = redirectType;
                RedirectToSourceConverter = redirectToSourceConverter;
            }

            public async Task<TSource> ToSourceAsync<TSource> (object obj)
            {
                return (TSource)await RedirectToSourceConverter.ConvertAsync(obj);
            }

            public TSource ToSource<TSource> (object obj)
            {
                return (TSource)RedirectToSourceConverter.Convert(obj);
            }
        }

        private ProjectResources projectResources;
        private Dictionary<Type, TypeRedirector> redirectors = new Dictionary<Type, TypeRedirector>();

        public ProjectResourceProvider ()
        {
            projectResources = ProjectResources.Get();
        }

        public override bool SupportsType<T> () => true;

        public void AddRedirector<TSource, TRedirect> (IConverter<TRedirect, TSource> redirectToSourceConverter)
        {
            var sourceType = typeof(TSource);
            if (!redirectors.ContainsKey(sourceType))
            {
                var redirector = new TypeRedirector(sourceType, typeof(TRedirect), redirectToSourceConverter);
                redirectors.Add(redirector.SourceType, redirector);
            }
        }

        protected override LoadResourceRunner<T> CreateLoadResourceRunner<T> (Resource<T> resource)
        {
            return new ProjectResourceLoader<T>(resource, redirectors.ContainsKey(typeof(T)) ? redirectors[typeof(T)] : null, LogMessage);
        }

        protected override LocateResourcesRunner<T> CreateLocateResourcesRunner<T> (string path)
        {
            return new ProjectResourceLocator<T>(path, projectResources);
        }

        protected override void UnloadResourceBlocking (Resource resource)
        {
            // TODO: We shouldn't destroy asset objects, but it's impossible (?) to tell if the object is an asset.
        }

        protected override Task UnloadResourceAsync (Resource resource)
        {
            // TODO: Support async unloading (?).
            UnloadResourceBlocking(resource);
            return Task.CompletedTask;
        }

        protected override Resource<T> LoadResourceBlocking<T> (string path)
        {
            var resource = new Resource<T>(path);
            var redirector = redirectors.ContainsKey(typeof(T)) ? redirectors[typeof(T)] : null;

            var resourceType = redirector != null ? redirector.RedirectType : typeof(T);
            var obj = Resources.Load(resource.Path, resourceType);
            resource.Object = redirector != null ? redirector.ToSource<T>(obj) : (T)obj;
            return resource;
        }

        protected override IEnumerable<Resource<T>> LocateResourcesBlocking<T> (string path)
        {
            return ProjectResourceLocator<T>.LocateProjectResources(path, projectResources);
        }

        protected override IEnumerable<Folder> LocateFoldersBlocking (string path)
        {
            return ProjectFolderLocator.LocateProjectFolders(path, projectResources);
        }

        protected override LocateFoldersRunner CreateLocateFoldersRunner (string path)
        {
            return new ProjectFolderLocator(path, projectResources);
        }
    }
}
