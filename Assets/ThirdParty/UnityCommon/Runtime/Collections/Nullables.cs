using System;
using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Simulates <see cref="System.Nullable"/> while keeping support for Unity serialization.
    /// <see cref="Value"/> won't return null when not set; always check for <see cref="HasValue"/>.
    /// </summary>
    [Serializable]
    public class Nullable<T>
    {
        public T Value { get { return value; } set { HasValue = true; this.value = value; } }
        public bool HasValue { get { return hasValue; } set { hasValue = value; } }

        [SerializeField] private T value;
        [SerializeField] private bool hasValue;

        public void Reset ()
        {
            Value = default(T);
            HasValue = false;
        }
    }

    [Serializable] public class NullableInt : Nullable<int> { }
    [Serializable] public class NullableFloat : Nullable<float> { }
    [Serializable] public class NullableString : Nullable<string> { }
    [Serializable] public class NullableBool : Nullable<bool> { }
    [Serializable] public class NullableVector3 : Nullable<Vector3> { }
    [Serializable] public class NullableVector2 : Nullable<Vector2> { }
    [Serializable] public class NullableQuaternion : Nullable<Quaternion> { }
}
