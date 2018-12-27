using System.Collections.Generic;
using System.Linq;

namespace UnityCommon
{
    public static class ResourceUtils
    {
        /// <summary>
        /// Given the path to the parent folder, returns all the unique resource paths inside that folder.
        /// </summary>
        public static IEnumerable<string> LocateResourcePathsAtFolder (this IEnumerable<string> source, string parentFolderPath)
        {
            parentFolderPath = parentFolderPath ?? string.Empty;
            if (string.IsNullOrWhiteSpace(parentFolderPath))
                return source.Where(p => !p.Contains("/") || string.IsNullOrEmpty(p.GetBeforeLast("/")));
            return source.Where(p => p.Contains("/") && (p.GetBeforeLast("/").Equals(parentFolderPath) || p.GetBeforeLast("/").Equals("/" + parentFolderPath)));
        }

        /// <summary>
        /// Given the path to the parent folder, returns all the unique folder paths inside that folder.
        /// </summary>
        public static IEnumerable<string> LocateFolderPathsAtFolder (this IEnumerable<string> source, string parentFolderPath)
        {
            parentFolderPath = parentFolderPath ?? string.Empty;
            if (parentFolderPath.StartsWithFast("/")) parentFolderPath = parentFolderPath.GetAfterFirst("/") ?? string.Empty;
            if (!parentFolderPath.EndsWithFast("/")) parentFolderPath += "/";
            return source.Where(p => p.StartsWithFast(parentFolderPath) && p.GetAfterFirst(parentFolderPath).Contains("/"))
                .Select(p => parentFolderPath + p.GetBetween(parentFolderPath, "/")).Distinct();
        }
    }
}
