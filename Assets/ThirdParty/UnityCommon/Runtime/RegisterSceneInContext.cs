using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Should be placed in scenes to proprely register objects with [RegisterInContext] attribute.
    /// Temp (hopefully) hack; waiting for [RuntimeInitializeOnLoadMethod]-esque thingy to support per-scene trigger.
    /// </summary>
    [ScriptOrder(-9999)]
    public class RegisterSceneInContext : MonoBehaviour
    {
        private void Awake ()
        {
            Context.RegisterSceneObjects();
        }

        #if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/Register In Context", false, 0)]
        public static void CreateRegisterSceneObject ()
        {
            new GameObject("RegisterInContext").AddComponent<RegisterSceneInContext>();
        }
        #endif
    }
}
