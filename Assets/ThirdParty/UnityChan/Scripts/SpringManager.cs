using UnityEngine;
using System.Collections;

namespace UnityChan
{
    public class SpringManager : MonoBehaviour
    {
        public float dynamicRatio = 1.0f;
        public float stiffnessForce;
        public AnimationCurve stiffnessCurve;
        public float dragForce;
        public AnimationCurve dragCurve;
        public SpringBone[] springBones;

        private void LateUpdate ()
        {
            if (dynamicRatio != 0.0f)
                for (int i = 0; i < springBones.Length; i++)
                    if (dynamicRatio > springBones[i].threshold)
                        springBones[i].UpdateSpring();
        }
    }
}
