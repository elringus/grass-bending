using System;
using UnityEngine;

namespace GrassBending
{
    public abstract class MonoBehaviourGrassBender : MonoBehaviour, IGrassBender, IEquatable<MonoBehaviourGrassBender>
    {
        public Vector3 Position => transform.position;
        public float BendRadius => bendRadius;
        public int Priority => priority;

        [Tooltip("Radius of the grass bending sphere."), Range(0.1f, 10f)]
        [SerializeField] private float bendRadius = 1f;
        [Tooltip("When concurrent bend sources limit is exceeded, benders with lower priority values will be served first.")]
        [SerializeField] private int priority = 0;

        public bool Equals (MonoBehaviourGrassBender other)
        {
            if (other is null) return false;
            return other.GetInstanceID() == GetInstanceID();
        }
    }
}
