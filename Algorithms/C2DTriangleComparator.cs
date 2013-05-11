using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    /// <summary>
    /// Comparator for C2DTriangle
    /// </summary>
    public class C2DTriangleComparator : IEqualityComparer<C2DTriangle>
    {
        /// <summary>
        /// Compares two triangles based on their points
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(C2DTriangle x, C2DTriangle y)
        {
            return x.p1.PointEqualTo(y.p1) && x.p2.PointEqualTo(y.p2) && x.p3.PointEqualTo(y.p3);
        }

        //Custom hashcode
        public int GetHashCode(C2DTriangle obj)
        {
            int result = 37; // prime

            result *= 397; // also prime (see note)
            result += obj.p1.ToString().GetHashCode();

            result *= 397;
            result += obj.p2.ToString().GetHashCode();

            result *= 397;
            result += obj.p3.ToString().GetHashCode();

            return result;
        }
    }
}
