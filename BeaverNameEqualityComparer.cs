using System.Collections.Generic;

namespace NameThatBeaver
{
    public class BeaverNameEqualityComparer : IEqualityComparer<BeaverName>
    {
        public bool Equals(BeaverName x, BeaverName y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(BeaverName obj)
        {
            return obj.GetHashCode();
        }
    }
}