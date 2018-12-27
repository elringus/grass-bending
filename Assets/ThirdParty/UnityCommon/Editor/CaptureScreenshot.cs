using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityCommon
{
    public static class CaptureScreenshot
    {
        private const string fileName = "EditorScreenshot#";
        private const int startIndex = 0;

        [MenuItem("Help/Capture Screenshot")]
        private static void CountLines ()
        {
            var index = startIndex;
            var projectPath = Application.dataPath.Replace("/Assets", string.Empty);
            var dirPath = Path.Combine(projectPath, "Screenshots");
            var filePath = string.Empty;
            Directory.CreateDirectory(dirPath);

            do
            {
                index++;
                filePath = Path.Combine(dirPath, $"{fileName}{index:000}.png");
            }
            while (File.Exists(filePath));

            ScreenCapture.CaptureScreenshot(filePath);
        }
    }
}
