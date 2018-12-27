#if UNITY_WEBGL && !UNITY_EDITOR
using System.Runtime.InteropServices;

namespace UnityCommon
{
    public static class WebGLExtensions
    {
        /// <summary>
        /// Calls FS.syncfs in native js.
        /// </summary>
        [DllImport("__Internal")]
        public static extern void SyncFs ();
    }
}
#endif
