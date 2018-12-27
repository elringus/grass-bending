using UnityEngine;

namespace UnityCommon
{
    public class ActiveWhenVisible : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] affectedScripts = null;

        private void Start ()
        {
            SetEnabled(false);
        }

        private void OnBecameVisible ()
        {
            SetEnabled(true);
        }

        private void OnBecameInvisible ()
        {
            SetEnabled(false);
        }

        private void SetEnabled (bool isEnabled)
        {
            if (affectedScripts == null) return;

            var length = affectedScripts.Length;
            for (int i = 0; i < length; i++)
                affectedScripts[i].enabled = isEnabled;
        }
    }
}
