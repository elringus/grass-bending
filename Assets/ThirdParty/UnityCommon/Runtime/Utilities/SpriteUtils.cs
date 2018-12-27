using System;
using UnityEngine;

namespace UnityCommon
{
    public static class SpriteUtils
    {
        public static void SetOpacity (this SpriteRenderer spriteRenderer, float opacity)
        {
            Debug.Assert(spriteRenderer != null);
            var spriteColor = spriteRenderer.color;
            spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, opacity);
        }

        public static bool IsTransparent (this SpriteRenderer spriteRenderer)
        {
            Debug.Assert(spriteRenderer != null);
            return Mathf.Approximately(spriteRenderer.color.a, 0f);
        }

        public static bool IsOpaque (this SpriteRenderer spriteRenderer)
        {
            Debug.Assert(spriteRenderer != null);
            return Mathf.Approximately(spriteRenderer.color.a, 1f);
        }
    }
}
