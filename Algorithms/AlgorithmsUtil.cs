using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class AlgorithmsUtil
    {
        /// <summary>
        /// Creates a random convex polygon. Note the result is probably much smaller than the number of points given.
        /// </summary>
        /// <param name="generationSizeMin"></param>
        /// <param name="generationSizeMax"></param>
        /// <param name="boundarySize"></param>
        /// <returns></returns>
        public static C2DPolygon RandomConvexPolygon(int generationSizeMin, int generationSizeMax, double boundarySize = 1)
        {
            var boundRect = new C2DRect(0, boundarySize, boundarySize, 0);
            var randomPol = new C2DPolygon();
            randomPol.CreateRandom(boundRect, generationSizeMin, generationSizeMax);
            var hull = new C2DPolygon();
            hull.CreateConvexHull(randomPol);
            return hull;
        }
    }
}
