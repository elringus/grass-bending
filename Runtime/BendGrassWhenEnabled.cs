using UnityEngine;

namespace GrassBending
{
    /// <summary>
    /// When attached to a <see cref="GameObject"/>, will trigger grass bending while enabled.
    /// </summary>
    [DisallowMultipleComponent]
    public class BendGrassWhenEnabled : MonoBehaviourGrassBender
    {
        private void OnEnable ()
        {
            GrassBendingManager.AddBender(this);
        }

        private void OnDisable ()
        {
            GrassBendingManager.RemoveBender(this);
        }
    }
}
