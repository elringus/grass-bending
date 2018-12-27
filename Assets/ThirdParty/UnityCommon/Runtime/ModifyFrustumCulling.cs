using UnityEngine;

namespace UnityCommon
{
    [RequireComponent(typeof(Camera))]
    public class ModifyFrustumCulling : MonoBehaviour
    {
        [SerializeField] private float modifyFactor = 1;

        private Camera modifiedCamera;

        private void Awake ()
        {
            modifiedCamera = GetComponent<Camera>();
        }

        private void OnPreCull ()
        {
            modifiedCamera.cullingMatrix = modifiedCamera.nonJitteredProjectionMatrix *
                Matrix4x4.Translate(Vector3.back * modifyFactor) * modifiedCamera.worldToCameraMatrix;
        }
    }
}
