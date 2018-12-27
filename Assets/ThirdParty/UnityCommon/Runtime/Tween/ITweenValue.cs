
namespace UnityCommon
{
    public interface ITweenValue
    {
        bool IsTimeScaleIgnored { get; }
        bool IsTargetValid { get; }
        float TweenDuration { get; }
        void TweenValue (float tweenPercent);
    }
}
