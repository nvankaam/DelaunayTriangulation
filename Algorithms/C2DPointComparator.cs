using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    /// <summary>
    /// Comparator for C2DPoint
    /// </summary>
    public class C2DPointComparator : IEqualityComparer<C2DPoint>
    {

        public bool Equals(C2DPoint x, C2DPoint y)
        {
            return x.PointEqualTo(y);
        }

        //Custom hashcode
        public int GetHashCode(C2DPoint obj)
        {
            int result = 37; // prime

            result *= 397; // also prime (see note)
            result += obj.x.ToString().GetHashCode();

            result *= 397;
            result += obj.y.ToString().GetHashCode();

            return result;
        }
    }
}
