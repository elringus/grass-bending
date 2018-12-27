using System.Collections;
using UnityEngine;

namespace UnityCommon
{
    public class CameraShaker : MonoBehaviour
    {
        public bool IsShaking { get { return Time.time < shakeEndTime; } }

        [SerializeField] private float shakeAmount = 10f;
        [SerializeField] private float shakeDuration = .5f;
        [SerializeField] private bool smooth = true;
        [SerializeField] private float smoothAmount = 1f;

        private float shakeStartTime;
        private float shakeEndTime = -1f;
        private float currentShakeAmount;
        private Vector3 initialLocalEuler;

        [ContextMenu("Shake")]
        public void Shake ()
        {
            Shake(shakeAmount, shakeDuration);
        }

        public void Shake (float amount, float duration)
        {
            currentShakeAmount = amount;
            shakeStartTime = Time.time;

            if (!IsShaking)
            {
                shakeEndTime = shakeStartTime + duration;
                initialLocalEuler = transform.localEulerAngles;
                StartCoroutine(ShakeRoutine());
            }
            else shakeEndTime += duration;
        }

        private IEnumerator ShakeRoutine ()
        {
            while (IsShaking)
            {
                var shakeProgress = (Time.time - shakeStartTime) / (shakeEndTime - shakeStartTime);
                var rotationAmount = initialLocalEuler + Random.insideUnitSphere * currentShakeAmount * shakeProgress;
                rotationAmount.z = initialLocalEuler.z;

                if (smooth) transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationAmount), Time.deltaTime / smoothAmount);
                else transform.localRotation = Quaternion.Euler(rotationAmount);

                yield return null;
            }

            transform.localEulerAngles = initialLocalEuler;
        }
    }
}
