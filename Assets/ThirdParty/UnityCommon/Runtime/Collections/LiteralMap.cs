using System;
using System.Collections.Generic;

namespace UnityCommon
{
    /// <summary>
    /// Dictionary with case-insensitive string keys.
    /// </summary>
    public class LiteralMap<TValue> : Dictionary<string, TValue>
    {
        public LiteralMap () : base(StringComparer.OrdinalIgnoreCase) { }
    }

    /// <summary>
    /// A serializable version of <see cref="LiteralMap{TValue}"/> with <see cref="string"/> values.
    /// </summary>
    [Serializable]
    public class SerializableLiteralStringMap : SerializableMap<string, string>
    {
        public SerializableLiteralStringMap () : base(StringComparer.OrdinalIgnoreCase) { }
    }
}
