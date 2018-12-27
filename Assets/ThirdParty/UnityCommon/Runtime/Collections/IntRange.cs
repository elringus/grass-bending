using UnityEngine;

namespace UnityCommon
{
    /// <summary>
    /// Represents an integer range starting with <see cref="StartIndex"/> and ending with <see cref="EndIndex"/>.
    /// Both endpoints are considered to be included.
    /// </summary>
    [System.Serializable]
    public struct IntRange
    {
        public int StartIndex => startIndex;
        public int EndIndex => endIndex;

        [SerializeField] private int startIndex;
        [SerializeField] private int endIndex;

        public IntRange (int startIndex, int endIndex)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }

        public bool Contains (int index)
        {
            return index >= StartIndex && index <= EndIndex;
        }
    }
}
