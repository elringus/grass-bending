using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityCommon
{
    public class ProjectResourceLocator<TResource> : LocateResourcesRunner<TResource> where TResource : UnityEngine.Object
    {
        public string ResourcesPath { get; private set; }

        private ProjectResources projectResources;

        public ProjectResourceLocator (string resourcesPath, ProjectResources projectResources)
        {
            ResourcesPath = resourcesPath ?? string.Empty;
            this.projectResources = projectResources;
        }

        public override async Task Run ()
        {
            await base.Run();

            LocatedResources = LocateProjectResources(ResourcesPath, projectResources);

            HandleOnCompleted();
        }

        public static List<Resource<TResource>> LocateProjectResources (string path, ProjectResources projectResources)
        {
            return projectResources.ResourcePaths.LocateResourcePathsAtFolder(path).Select(p => new Resource<TResource>(p)).ToList();
        }
    }
}
