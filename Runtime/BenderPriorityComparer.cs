using System.Collections.Generic;

namespace GrassBending
{
    public sealed class BenderPriorityComparer : IComparer<IGrassBender>
    {
        public int Compare (IGrassBender x, IGrassBender y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            return x.Priority.CompareTo(y.Priority);
        }
    }
}
