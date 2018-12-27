using System.Collections.Generic;

namespace UnityCommon
{
    /// <summary>
    /// Represents a container for two generic items.
    /// </summary>
    /// <typeparam name="T1">First item type.</typeparam>
    /// <typeparam name="T2">Second item type.</typeparam> 
    [System.Serializable]
    public class Pair<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        private static readonly IEqualityComparer<T1> item1Comparer = EqualityComparer<T1>.Default;
        private static readonly IEqualityComparer<T2> item2Comparer = EqualityComparer<T2>.Default;

        public Pair (T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override string ToString ()
        {
            return string.Format("<{0}, {1}>", Item1, Item2);
        }

        public override int GetHashCode ()
        {
            int hash = 17;
            hash = hash * 23 + Item1.GetHashCode();
            hash = hash * 23 + Item2.GetHashCode();
            return hash;
        }

        public override bool Equals (object obj)
        {
            var other = obj as Pair<T1, T2>;
            if (object.ReferenceEquals(other, null)) return false;
            else return item1Comparer.Equals(Item1, other.Item1) && item2Comparer.Equals(Item2, other.Item2);
        }

        public static bool operator == (Pair<T1, T2> a, Pair<T1, T2> b)
        {
            if (IsNull(a) && !IsNull(b)) return false;
            if (!IsNull(a) && IsNull(b)) return false;
            if (IsNull(a) && IsNull(b)) return true;

            return a.Item1.Equals(b.Item1) && a.Item2.Equals(b.Item2);
        }

        public static bool operator != (Pair<T1, T2> a, Pair<T1, T2> b)
        {
            return !(a == b);
        }

        private static bool IsNull (object obj)
        {
            return object.ReferenceEquals(obj, null);
        }
    }
}
   