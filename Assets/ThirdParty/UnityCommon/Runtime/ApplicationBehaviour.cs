using UnityEngine;

namespace UnityCommon
{
    public class ApplicationBehaviour : MonoBehaviour
    {
        public static ApplicationBehaviour Singleton => singleton ?? CreateSingleton();

        private static ApplicationBehaviour singleton;

        private static ApplicationBehaviour CreateSingleton ()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("ApplicationBehaviour doesn't work at edit time.");
                return null;
            }

            singleton = new GameObject("ApplicationBehaviour").AddComponent<ApplicationBehaviour>();
            singleton.gameObject.hideFlags = HideFlags.DontSave;
            DontDestroyOnLoad(singleton.gameObject);
            return singleton;
        }

        private void OnDestroy ()
        {
            singleton = null;
        }
    }
}
