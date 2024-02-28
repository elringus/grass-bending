using UnityEngine;

namespace GrassBending
{
    public class MonoBehaviourGrassBender : MonoBehaviour, IGrassBender
    {
        public virtual Vector3 Position => transform.position;
        public virtual float BendRadius { get => bendRadius; set => bendRadius = value; }
        public virtual int Priority { get => priority; set => priority = value; }

        [Tooltip("Radius of the grass bending sphere."), Range(0.1f, 10f)]
        [SerializeField] private float bendRadius = 1f;
        [Tooltip("When concurrent bend sources limit is exceeded, benders with lower priority values will be served first.")]
        [SerializeField] private int priority;
    }
}
