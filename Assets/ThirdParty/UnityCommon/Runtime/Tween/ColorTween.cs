using System;
using UnityEngine;

namespace UnityCommon
{
    public enum ColorTweenMode { All, RGB, Alpha }

    public struct ColorTween : ITweenValue
    {
        public event Action<Color> OnColorTween;

        public Color StartColor { get; set; }
        public Color TargetColor { get; set; }
        public ColorTweenMode TweenMode { get; set; }
        public float TweenDuration { get; set; }
        public bool IsTimeScaleIgnored { get; set; }
        public bool IsTargetValid { get { return OnColorTween != null; } }

        public ColorTween (Color from, Color to, ColorTweenMode mode, float time, Action<Color> onTween, bool ignoreTimeScale = false)
        {
            StartColor = from;
            TargetColor = to;
            TweenMode = mode;
            TweenDuration = time;
            IsTimeScaleIgnored = ignoreTimeScale;
            OnColorTween = onTween;
        }

        public void TweenValue (float tweenPercent)
        {
            if (!IsTargetValid) return;

            var newColor = Color.Lerp(StartColor, TargetColor, tweenPercent);

            if (TweenMode == ColorTweenMode.Alpha)
            {
                newColor.r = StartColor.r;
                newColor.g = StartColor.g;
                newColor.b = StartColor.b;
            }
            else if (TweenMode == ColorTweenMode.RGB)
            {
                newColor.a = StartColor.a;
            }

            OnColorTween.Invoke(newColor);
        }

    }
}
