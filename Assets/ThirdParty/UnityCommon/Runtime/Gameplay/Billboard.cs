using UnityEngine;

namespace UnityCommon
{
    public class Billboard : MonoBehaviour
    {
        private Transform cameraTransform;
        private Transform myTransform;

        private void Awake ()
        {
            cameraTransform = Camera.main.transform;
            myTransform = transform;
        }

        private void Start ()
        {
            enabled = false;
        }

        public void SetLookCamera (Camera camera)
        {
            cameraTransform = camera.transform;
        }

        private void LateUpdate ()
        {
            myTransform.forward = cameraTransform.forward;
        }

        private void OnBecameVisible ()
        {
            enabled = true;
        }

        private void OnBecameInvisible ()
        {
            enabled = false;
        }
    }
}
