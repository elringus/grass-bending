using System;
using UnityEngine;

namespace UnityCommon
{
    public class ProxyBehaviour : MonoBehaviour
    {
        public event Action OnBehaviourAwake;
        public event Action OnBehaviourEnable;
        public event Action OnBehaviourStart;
        public event Action OnBehaviourUpdate;
        public event Action OnBehaviourDisable;
        public event Action OnBehaviourDestroy;

        private void Awake ()
        {
            OnBehaviourAwake.SafeInvoke();
        }

        private void OnEnable ()
        {
            OnBehaviourEnable.SafeInvoke();
        }

        private void Start ()
        {
            OnBehaviourStart.SafeInvoke();
        }

        private void Update ()
        {
            OnBehaviourUpdate.SafeInvoke();
        }

        private void OnDisable ()
        {
            OnBehaviourDisable.SafeInvoke();
        }

        private void OnDestroy ()
        {
            OnBehaviourDestroy.SafeInvoke();
        }
    }
}
