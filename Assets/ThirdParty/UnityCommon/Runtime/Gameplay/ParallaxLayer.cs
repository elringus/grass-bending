using UnityEngine;

namespace UnityCommon
{
    public class ParallaxLayer : MonoBehaviour
    {
        public Camera Camera { get { return cameraComponent; } }
        public float ParallaxFactor { get { return parallaxFactor; } }

        private Transform cameraTrs;
        private float initialOffset;

        [SerializeField] private Camera cameraComponent = null;
        [Range(0f, 1f)]
        [SerializeField] private float parallaxFactor = 1f;

        private void Awake ()
        {
            cameraTrs = Camera ? Camera.transform : Camera.main.transform;
            Debug.Assert(cameraTrs, "Assign required objects to ParallaxLayer.");
            initialOffset = (transform.position.x - cameraTrs.position.x) / ParallaxFactor;
        }

        private void Update ()
        {
            var targetPosX = cameraTrs.position.x + initialOffset;
            transform.SetPosX(targetPosX * ParallaxFactor);
        }
    }
}
