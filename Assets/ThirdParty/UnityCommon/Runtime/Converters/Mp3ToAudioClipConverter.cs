using NLayer;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Converts <see cref="T:byte[]"/> raw data of a .mp3 audio file to <see cref="AudioClip"/>.
    /// </summary>
    public class Mp3ToAudioClipConverter : IRawConverter<AudioClip>
    {
        public RawDataRepresentation[] Representations { get { return new RawDataRepresentation[] {
            new RawDataRepresentation(".mp3", "audio/mpeg"),    // RFC standard: https://tools.ietf.org/html/rfc3003
            new RawDataRepresentation(".mp3", "audio/mp3")      //... but Chrome uses this one when uploading an mp3.
        }; } }

        public AudioClip Convert (byte[] obj)
        {
            var mpegFile = new MpegFile(new MemoryStream(obj));
            var audioClip = AudioClip.Create("Generated MP3 Audio", (int)mpegFile.SampleCount, mpegFile.Channels, mpegFile.SampleRate, false);

            // AudioClip.SetData with offset is not supported on WebGL, thus we can't use buffering while decoding.
            // Issue: https://trello.com/c/iWL6eBrV/82-webgl-audio-resources-limitation
            #if UNITY_WEBGL && !UNITY_EDITOR
            DecodeMpeg(mpegFile, audioClip);
            #else
            DecodeMpegBuffered(mpegFile, audioClip);
            #endif

            mpegFile.Dispose();

            return audioClip;
        }

        public async Task<AudioClip> ConvertAsync (byte[] obj)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            var mpegFile = new MpegFile(new MemoryStream(obj));
            var audioClip = AudioClip.Create("Generated MP3 Audio", (int)mpegFile.SampleCount, mpegFile.Channels, mpegFile.SampleRate, false);

            // AudioClip.SetData with offset is not supported on WebGL, thus we can't use buffering while decoding.
            // Issue: https://trello.com/c/iWL6eBrV/82-webgl-audio-resources-limitation
            #if UNITY_WEBGL && !UNITY_EDITOR
            await DecodeMpegAsync(mpegFile, audioClip);
            #else
            await DecodeMpegBufferedAsync(mpegFile, audioClip);
            #endif

            mpegFile.Dispose();

            return audioClip;
        }

        public object Convert (object obj) => Convert(obj as byte[]);

        public async Task<object> ConvertAsync (object obj) => await ConvertAsync(obj as byte[]);

        private void DecodeMpeg (MpegFile mpegFile, AudioClip audioClip)
        {
            var samplesCount = (int)mpegFile.SampleCount * mpegFile.Channels;
            var samples = new float[samplesCount];
            mpegFile.ReadSamples(samples, 0, samplesCount);
            audioClip.SetData(samples, 0);
        }

        private async Task DecodeMpegAsync (MpegFile mpegFile, AudioClip audioClip)
        {
            var samplesCount = (int)mpegFile.SampleCount * mpegFile.Channels;
            var samples = new float[samplesCount];
            await Task.Run(() => mpegFile.ReadSamples(samples, 0, samplesCount));
            audioClip.SetData(samples, 0);
        }

        private void DecodeMpegBuffered (MpegFile mpegFile, AudioClip audioClip)
        {
            var bufferLength = mpegFile.SampleRate;
            var samplesBuffer = new float[bufferLength];
            var sampleOffset = 0;
            while (mpegFile.Position < mpegFile.Length)
            {
                var samplesRead = mpegFile.ReadSamples(samplesBuffer, 0, bufferLength);
                if (samplesRead < bufferLength) Array.Resize(ref samplesBuffer, samplesRead);
                audioClip.SetData(samplesBuffer, (sampleOffset / sizeof(float)) * mpegFile.Channels);
                if (samplesRead < bufferLength) break;
                sampleOffset += samplesRead;
            }
        }

        private async Task DecodeMpegBufferedAsync (MpegFile mpegFile, AudioClip audioClip)
        {
            var bufferLength = mpegFile.SampleRate;
            var samplesBuffer = new float[bufferLength];
            var sampleOffset = 0;
            while (mpegFile.Position < mpegFile.Length)
            {
                var samplesRead = await Task.Run(() => mpegFile.ReadSamples(samplesBuffer, 0, bufferLength));
                if (samplesRead < bufferLength) Array.Resize(ref samplesBuffer, samplesRead);
                audioClip.SetData(samplesBuffer, (sampleOffset / sizeof(float)) * mpegFile.Channels);
                if (samplesRead < bufferLength) break;
                sampleOffset += samplesRead;
            }
        }
    }
}
