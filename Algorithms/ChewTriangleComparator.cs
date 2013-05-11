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
    public class ChewTriangleComparator : IEqualityComparer<ChewTriangle>
    {
        /// <summary>
        /// Compares two triangles based on their points
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ChewTriangle x, ChewTriangle y)
        {
            return x.P1 == y.P1 && x.P2 == y.P2 && x.P3 == y.P3;
        }

        //Custom hashcode
        public int GetHashCode(ChewTriangle obj)
        {
            int result = 37; // prime

            result *= 397; // also prime (see note)
            result += obj.P1;

            result *= 397;
            result += obj.P2;

            result *= 397;
            result += obj.P3;

            return result;
        }
    }
}
