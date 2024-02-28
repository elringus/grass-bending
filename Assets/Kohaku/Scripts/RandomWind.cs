using UnityEngine;
using System.Collections;

namespace UnityChan
{
    public class RandomWind : MonoBehaviour
    {
        private SpringBone[] springBones;
        public bool isWindActive = false;

        private bool isMinus = false;
        public float threshold = 0.5f;
        public float interval = 5.0f;
        public float windPower = 1.0f;
        public float gravity = 0.98f;
        public bool isGUI = true;

        void Start ()
        {
            springBones = GetComponent<SpringManager>().springBones;
            StartCoroutine(nameof(RandomChange));
        }

        void Update ()
        {
            if (!isWindActive) return;
            Vector3 force = Vector3.zero;
            if (isMinus) force = new Vector3(Mathf.PerlinNoise(Time.time, 0.0f) * windPower * -0.001f, gravity * -0.001f, 0);
            else force = new Vector3(Mathf.PerlinNoise(Time.time, 0.0f) * windPower * 0.001f, gravity * -0.001f, 0);
            for (int i = 0; i < springBones.Length; i++)
                springBones[i].springForce = force;
        }

        IEnumerator RandomChange ()
        {
            while (true)
            {
                float seed = Random.Range(0.0f, 1.0f);
                if (seed > threshold) isMinus = true;
                else isMinus = false;
                yield return new WaitForSeconds(interval);
            }
        }
    }
}
