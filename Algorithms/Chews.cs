using GeoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class Chews
    {
        /// <summary>
        /// The input polygon for chews algorithm
        /// </summary>
        public C2DPolygon InputPolygon {get; set;}
        Random RandomGenerator { get; set; }

        public Chews() {
            RandomGenerator = new Random();
        }

        /// <summary>
        /// Runs chews algorithm
        /// </summary>
        public void run()
        {
            if (!InputPolygon.IsConvex())
                throw new InvalidOperationException("Chews algorithm was called on a non-convex polygon");

            var points = new List<C2DPoint>();
            InputPolygon.GetPointsCopy(points);
            var chewPoints = points.ConvertAll(o => {
                    var result = new ChewPoint();
                    result.x = o.x;
                    result.y = o.y;
                    return result;
                }
            );

            ChewRecur(chewPoints);

        }

        public IList<C2DTriangle> ChewRecur(IList<ChewPoint> points)
        {
            if (points.Count <= 3)
            {
                if (points.Count < 3)
                    throw new InvalidOperationException("Less than two points in the given triangle");
                var singleTriangle = new List<C2DTriangle>();
                singleTriangle.Add(new C2DTriangle(points[0],points[1],points[2]));
                return singleTriangle;
            }

            int randIndex = RandomGenerator.Next(0, points.Count);
            Debug.WriteLine("Random nr: " + randIndex);

            var nodeP = points[randIndex];
            var nodeQ = points[(randIndex + points.Count - 1) % points.Count];
            var nodeR = points[(randIndex + 1) % points.Count];

            

            points.RemoveAt(randIndex);
            var triangles = ChewRecur(points);
            var pqrTriangle = new C2DTriangle(nodeP, nodeQ, nodeR);
            //Store reference to the triangle on the points, for later use
            nodeP.triangles.Add(pqrTriangle);
            nodeQ.triangles.Add(pqrTriangle);
            nodeR.triangles.Add(pqrTriangle);

            var filtered = triangles.Where(o => {
                    var center = o.GetCircumCentre();
                    return center.Distance(nodeP) <= center.Distance(o.p1);
                }
            );




            return triangles;
        }
    }

   
}
