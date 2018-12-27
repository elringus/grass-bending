using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace UnityCommon
{
    public class ProjectResourcesBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder => 100;

        private const string tempFolderPath = "Assets/TEMP_UNITY_COMMON/Resources";
        private const string assetPath = tempFolderPath + "/" + nameof(ProjectResources) + ".asset";

        public void OnPreprocessBuild (UnityEditor.Build.Reporting.BuildReport report)
        {
            var asset = ScriptableObject.CreateInstance<ProjectResources>();
            asset.LocateAllResources();
            EditorUtils.CreateFolderAsset(tempFolderPath);
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
        }

        public void OnPostprocessBuild (UnityEditor.Build.Reporting.BuildReport report)
        {
            AssetDatabase.DeleteAsset(tempFolderPath.GetBeforeLast("/"));
            AssetDatabase.SaveAssets();
        }
    }
}
