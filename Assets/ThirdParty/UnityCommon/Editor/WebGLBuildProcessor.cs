using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

namespace UnityCommon
{
    public class WebGLBuildProcessor
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild (BuildTarget target, string targetPath)
        {
            if (target != BuildTarget.WebGL) return;

            // Remove mobiles warning; based on: http://answers.unity.com/answers/1561748/view.html
            var path = Path.Combine(targetPath, "Build/UnityLoader.js");
            var text = File.ReadAllText(path);
            text = text.Replace("UnityLoader.SystemInfo.mobile", "false");
            File.WriteAllText(path, text);
        }
    }
}
