using UnityEngine;

namespace UnityCommon
{
    public static class WebUtils
    {
        public static AudioType EvaluateAudioTypeFromMime (string mimeType)
        {
            switch (mimeType)
            {
                case "audio/aiff": return AudioType.AIFF;
                case "audio/mpeg": return AudioType.MPEG;
                case "audio/mpeg3": return AudioType.MPEG;
                case "audio/mp3": return AudioType.MPEG;
                case "audio/ogg": return AudioType.OGGVORBIS;
                case "video/ogg": return AudioType.OGGVORBIS;
                case "audio/wav": return AudioType.WAV;
                default: return AudioType.UNKNOWN;
            }
        }
    }
}
