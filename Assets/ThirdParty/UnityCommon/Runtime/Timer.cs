using System;
using UnityEngine;

namespace UnityCommon
{
    public class Timer : CoroutineRunner
    {
        public event Action OnLoop;

        public override bool CanBeInstantlyCompleted => true;
        public bool Loop { get; private set; }
        public bool IsTimeScaleIgnored { get; private set; }
        public float Duration { get; private set; }
        public float ElapsedTime { get; private set; }

        public Timer (float duration = 0f, bool loop = false, bool ignoreTimeScale = false,
            MonoBehaviour coroutineContainer = null, Action onCompleted = null, Action onLoop = null) : base(coroutineContainer)
        {
            Duration = duration;
            Loop = loop;
            IsTimeScaleIgnored = ignoreTimeScale;

            if (onCompleted != null) OnCompleted += onCompleted;
            if (onLoop != null) OnLoop += onLoop;
        }

        public void Run (float duration, bool loop = false, bool ignoreTimeScale = false)
        {
            ElapsedTime = 0f;
            Duration = duration;
            Loop = loop;
            IsTimeScaleIgnored = ignoreTimeScale;

            base.Run();
        }

        public override void Run () => Run(Duration, Loop, IsTimeScaleIgnored);

        public override void Stop ()
        {
            base.Stop();

            Loop = false;
        }

        protected override bool LoopCondition ()
        {
            return ElapsedTime < Duration;
        }

        protected override void OnCoroutineTick ()
        {
            base.OnCoroutineTick();

            ElapsedTime += IsTimeScaleIgnored ? Time.unscaledDeltaTime : Time.deltaTime;
        }

        protected override void HandleOnCompleted ()
        {
            if (Loop)
            {
                OnLoop.SafeInvoke();
                base.Stop();
                Run();
            }
            else base.HandleOnCompleted();
        }
    }
}
