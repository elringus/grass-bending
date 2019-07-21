using UnityEngine;

namespace GrassBending
{
    /// <summary>
    /// Implementation is able to bend grass using <see cref="GrassBendingManager"/>.
    /// </summary>
    public interface IGrassBender
    {
        /// <summary>
        /// Current bender position in world space.
        /// </summary>
        Vector3 Position { get; }
        /// <summary>
        /// Radius of the grass bending sphere.
        /// </summary>
        float BendRadius { get; }
        /// <summary>
        /// Bend source priority.
        /// </summary>
        int Priority { get; }
    }
}
