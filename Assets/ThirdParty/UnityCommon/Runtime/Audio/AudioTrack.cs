using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityCommon
{
    /// <summary>
    /// Represents and audio source with attached clip.
    /// </summary>
    public class AudioTrack
    {
        public string Name { get { return Clip.name; } }
        public AudioClip Clip { get; private set; }
        public AudioSource Source { get; private set; }
        public bool IsValid { get { return Clip && Source; } }
        public bool IsLooped { get { return IsValid ? Source.loop : false; } set { if (IsValid) Source.loop = value; } }
        public bool IsPlaying { get { return IsValid ? Source.isPlaying : false; } }
        public bool IsMuted { get { return IsValid ? Source.mute : false; } set { if (IsValid) Source.mute = value; } }
        public float Volume { get { return IsValid ? Source.volume : 0f; } set { if (IsValid) Source.volume = value; } }

        private Tweener<FloatTween> volumeTweener;
        private Timer stopTimer;

        public AudioTrack (AudioClip clip, AudioSource source, MonoBehaviour behaviourContainer = null,
            float volume = 1f, bool loop = false, AudioMixerGroup mixerGroup = null)
        {
            Clip = clip;
            Source = source;
            Source.clip = Clip;
            Source.volume = volume;
            Source.loop = loop;
            Source.outputAudioMixerGroup = mixerGroup;

            volumeTweener = new Tweener<FloatTween>(behaviourContainer);
            stopTimer = new Timer(coroutineContainer: behaviourContainer, onCompleted: Stop);
        }

        public void Play ()
        {
            if (!IsValid) return;
            CompleteAllRunners();

            Source.Play();
        }

        public async Task PlayAsync (float fadeInTime)
        {
            if (!IsValid) return;
            CompleteAllRunners();

            if (!IsPlaying) Play();
            var tween = new FloatTween(0, Volume, fadeInTime, volume => Volume = volume);
            await volumeTweener.RunAsync(tween);
        }

        public void Stop ()
        {
            if (!IsValid) return;
            CompleteAllRunners();

            Source.Stop();
        }

        public async Task StopAsync (float fadeOutTime)
        {
            if (!IsValid) return;
            CompleteAllRunners();

            var tween = new FloatTween(Volume, 0, fadeOutTime, volume => Volume = volume);
            stopTimer.Run(fadeOutTime);
            await volumeTweener.RunAsync(tween);
        }

        public async Task FadeAsync (float volume, float fadeTime)
        {
            if (!IsValid) return;
            CompleteAllRunners();

            var tween = new FloatTween(Volume, volume, fadeTime, v => Volume = v);
            await volumeTweener.RunAsync(tween);
        }

        private void CompleteAllRunners ()
        {
            if (volumeTweener.IsRunning)
                volumeTweener.CompleteInstantly();
            if (stopTimer.IsRunning)
                stopTimer.CompleteInstantly();
        }
    }
}
