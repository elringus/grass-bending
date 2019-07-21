using System.Collections.Generic;
using System.Linq;
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
            public System.Action OnUpdate { get; set; }
            private void Update () => OnUpdate?.Invoke();
        }

        private const int sourcesLimit = 16;
        private static readonly HashSet<IGrassBender> benders = new HashSet<IGrassBender>();
        private static readonly Vector4[] bendData = new Vector4[sourcesLimit];
        private static readonly int bendDataPropertyId = Shader.PropertyToID("_BendData");

        public static void AddBender (IGrassBender bender)
        {
            if (!benders.Add(bender)) return;

            // SortedSet generates garbage on enumeration, so hacking with linq here.
            var sortedBenders = benders.OrderBy(b => b.Priority).ToList();
            benders.Clear();
            benders.UnionWith(sortedBenders);
        }

        public static void RemoveBender (IGrassBender bender) => benders.Remove(bender);

        private static void ProcessBenders ()
        {
            var sourceIndex = 0;
            foreach (var bender in benders)
            {
                if (sourceIndex >= sourcesLimit) break;
                bendData[sourceIndex] = new Vector4(bender.Position.x, bender.Position.y, bender.Position.z, bender.BendRadius);
                sourceIndex++;
            }

            for (int i = sourceIndex; i < bendData.Length; i++)
                bendData[i] = Vector4.zero;

            Shader.SetGlobalVectorArray(bendDataPropertyId, bendData);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize ()
        {
            var objectName = nameof(GrassBendingManager);
            var gameObject = new GameObject(objectName);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(gameObject);

            var proxyBehaviour = gameObject.AddComponent<ProxyBehaviour>();
            proxyBehaviour.OnUpdate = ProcessBenders;
        }
    }
}
