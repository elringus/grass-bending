using System.Collections.Generic;
using UnityEngine;

namespace GrassBending
{
    /// <summary>
    /// Manages <see cref="IGrassBender"/> objects and provides bending data to the shader.
    /// </summary>
    public static class GrassBendingManager
    {
        private class ProxyBehaviour : MonoBehaviour
        {
            private void Update () => ApplyBending();
            private void OnDestroy () => benders.Clear();
        }

        private const int sourcesLimit = 16;
        private static readonly BenderPriorityComparer comparer = new BenderPriorityComparer();
        private static readonly List<IGrassBender> benders = new List<IGrassBender>();
        private static readonly Vector4[] bendData = new Vector4[sourcesLimit];
        private static readonly int bendDataPropertyId = Shader.PropertyToID("_BendData");

        public static IReadOnlyList<IGrassBender> GetBenders () => benders;

        public static void AddBender (IGrassBender bender)
        {
            // Sorted collections allocate garbage on enumeration; using list instead.
            // The list length is expected to be minimal, so the overhead is negligible.
            benders.Add(bender);
            benders.Sort(comparer);
        }

        public static bool RemoveBender (IGrassBender bender) => benders.Remove(bender);

        private static void ApplyBending ()
        {
            for (int i = 0; i < bendData.Length; i++)
                bendData[i] = GetBendDataAt(i);
            Shader.SetGlobalVectorArray(bendDataPropertyId, bendData);
        }

        private static Vector4 GetBendDataAt (int index)
        {
            if (index >= benders.Count) return Vector4.zero;
            var bender = benders[index];
            return new Vector4(bender.Position.x, bender.Position.y, bender.Position.z, bender.BendRadius);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize ()
        {
            var gameObject = new GameObject(nameof(GrassBendingManager));
            gameObject.AddComponent<ProxyBehaviour>();
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(gameObject);
        }
    }
}
