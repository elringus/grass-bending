using UnityEngine;

namespace UnityCommon
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float speed = 30f;
        [SerializeField] private Vector3 axis = new Vector3(1, 1, 1);

        private void Update ()
        {
            transform.Rotate(axis, speed * Time.deltaTime);
        }
    }
}
