using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnityCommon
{
    public class ProjectFolderLocator : LocateFoldersRunner
    {
        public string ResourcesPath { get; private set; }

        private ProjectResources projectResources;

        public ProjectFolderLocator (string resourcesPath, ProjectResources projectResources)
        {
            ResourcesPath = resourcesPath ?? string.Empty;
            this.projectResources = projectResources;
        }

        public override async Task Run ()
        {
            await base.Run();

            LocatedFolders = LocateProjectFolders(ResourcesPath, projectResources);

            HandleOnCompleted();
        }

        public static List<Folder> LocateProjectFolders (string path, ProjectResources projectResources)
        {
            return projectResources.ResourcePaths.LocateFolderPathsAtFolder(path).Select(p => new Folder(p)).ToList();
        }
    }
}
