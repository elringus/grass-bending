using System;
using UnityEngine;

namespace UnityCommon
{
    public struct VectorTween : ITweenValue
    {
        public event Action<Vector3> OnTween;

        public Vector3 StartValue { get; set; }
        public Vector3 TargetValue { get; set; }
        public float TweenDuration { get; set; }
        public bool IsTimeScaleIgnored { get; set; }
        public bool SmoothStep { get; set; }
        public bool IsTargetValid { get { return OnTween != null; } }

        public VectorTween (Vector3 from, Vector3 to, float time, Action<Vector3> onTween, bool ignoreTimeScale = false, bool smoothStep = false)
        {
            StartValue = from;
            TargetValue = to;
            TweenDuration = time;
            IsTimeScaleIgnored = ignoreTimeScale;
            SmoothStep = smoothStep;
            OnTween = onTween;
        }

        public void TweenValue (float tweenPercent)
        {
            if (!IsTargetValid) return;

            var newValue = new Vector3(
                TweenFloat(StartValue.x, TargetValue.x, tweenPercent),
                TweenFloat(StartValue.y, TargetValue.y, tweenPercent),
                TweenFloat(StartValue.z, TargetValue.z, tweenPercent)
            );

            OnTween.Invoke(newValue);
        }

        private float TweenFloat (float startValue, float targetValue, float tweenPercent)
        {
            return SmoothStep ? Mathf.SmoothStep(startValue, targetValue, tweenPercent) : Mathf.Lerp(startValue, targetValue, tweenPercent);
        }

    }
}
