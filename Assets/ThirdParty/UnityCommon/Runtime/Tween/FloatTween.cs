using System;
using UnityEngine;

namespace UnityCommon
{
    public struct FloatTween : ITweenValue
    {
        public event Action<float> OnFloatTween;

        public float StartValue { get; set; }
        public float TargetValue { get; set; }
        public float TweenDuration { get; set; }
        public bool IsTimeScaleIgnored { get; set; }
        public bool SmoothStep { get; set; }
        public bool IsTargetValid { get { return OnFloatTween != null; } }

        public FloatTween (float from, float to, float time, Action<float> onTween, bool ignoreTimeScale = false, bool smoothStep = false)
        {
            StartValue = from;
            TargetValue = to;
            TweenDuration = time;
            IsTimeScaleIgnored = ignoreTimeScale;
            SmoothStep = smoothStep;
            OnFloatTween = onTween;
        }

        public void TweenValue (float tweenPercent)
        {
            if (!IsTargetValid) return;

            var newValue = SmoothStep ? Mathf.SmoothStep(StartValue, TargetValue, tweenPercent) : Mathf.Lerp(StartValue, TargetValue, tweenPercent);
            OnFloatTween.Invoke(newValue);
        }

    }
}
