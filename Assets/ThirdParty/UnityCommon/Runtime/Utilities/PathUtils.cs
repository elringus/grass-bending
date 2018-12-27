using System.IO;
using UnityEngine;

namespace UnityCommon
{
    public static class PathUtils
    {
        /// <summary>
        /// Given an absolute path (eg, `C:\UnityProject\Assets\FooAsset.asset`),
        /// transforms it to a relative project asset path (eg, `Assets/FooAsset.asset`).
        /// </summary>
        public static string AbsoluteToAssetPath (string absolutePath)
        {
            absolutePath = absolutePath.Replace("\\", "/");
            return "Assets" + absolutePath.Replace(Application.dataPath, string.Empty);
        }

        /// <summary>
        /// Inovkes <see cref="Path.Combine(string[])"/> and replaces back slashes with forward slashes on the result.
        /// </summary>
        public static string Combine (params string[] paths)
        {
            return Path.Combine(paths)?.Replace("\\", "/");
        }
    }
}
