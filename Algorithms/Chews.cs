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

        /// <summary>
        /// The recursive method for chew's algorithm
        /// List is chosen above IList because we need foreach, and we only use default Lists anyway
        /// </summary>
        /// <param name="points">The points in the diagram.</param>
        /// <returns></returns>
        public List<C2DTriangle> ChewRecur(List<ChewPoint> points)
        {
            if (points.Count <= 3)
            {
                if (points.Count < 3)
                    throw new InvalidOperationException("Less than two points in the given triangle");
                var singleTriangle = new List<C2DTriangle>();
                singleTriangle.Add(new C2DTriangle(points[0],points[1],points[2]));
                return singleTriangle;
            }

            //Pick a random point (Step 1)
            int randIndex = RandomGenerator.Next(0, points.Count);
            Debug.WriteLine("Random nr: " + randIndex);

            var nodeP = points[randIndex];
            var nodeQ = points[(randIndex + points.Count - 1) % points.Count];
            var nodeR = points[(randIndex + 1) % points.Count];

            points.RemoveAt(randIndex);
            List<C2DTriangle> triangles = ChewRecur(points);
            var pqrTriangle = new C2DTriangle(nodeP, nodeQ, nodeR);

            //Store reference to the triangle on the points, for later use
            //TODO: Fix this on a better way
            nodeP.triangles.Add(pqrTriangle);
            nodeQ.triangles.Add(pqrTriangle);
            nodeR.triangles.Add(pqrTriangle);

            //All triangles whose circumcircle contains P.
            //TODO: Optimize this so it is in O(n)
            //TODO: Don't check the pqr triangle because we already know its within the range of P.
            var filtered = triangles.Where(o => {
                    var center = o.GetCircumCentre();
                    return center.Distance(nodeP) <= center.Distance(o.p1);
                }
            );
            //Remove the filtered list from the list
            filtered.ToList().ForEach(o => triangles.Remove(o));

            //Obtain all points
            var filteredPoints = points.Where(o =>
            {
                foreach (var t in o.triangles)
                {
                    if (triangles.Contains(t))
                        return true;
                }
                return false;
            });

            //Add the retriangulated version back to the list
            triangles.AddRange(ReTriangulate(filtered, filteredPoints, nodeP));

            return triangles;
        }

        /// <summary>
        /// Retriangulates the set of points with P as endpoint. 
        /// Note this only works for convex polygon points and assumes the points are sorted in the same order as they appear in the polygon!
        /// </summary>
        /// <param name="S"></param>
        /// <param name="selectedPoints"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        private IEnumerable<C2DTriangle> ReTriangulate(IEnumerable<C2DTriangle> S, IEnumerable<ChewPoint> selectedPoints, ChewPoint P)
        {
            Debug.WriteLine("Size of retriangulate set: " + S.Count()); //Debugging if its actually O(1)
            var result = new List<C2DTriangle>();

            //Remove all references to the current triangles
            foreach (var p in selectedPoints)
            {
                p.triangles.RemoveAll(o => S.Contains(o));
            }

            var pointStack = new Stack<ChewPoint>(selectedPoints);
            while (true)
            {
                var node1 = pointStack.Pop();
                if (node1 == null)
                    return result;
                if (node1 == P)
                    node1 = pointStack.Pop();
                var node2 = pointStack.Pop();
                if (node2 == P)
                    node2 = pointStack.Pop();
                if (node2 == null)
                    throw new InvalidOperationException("Invalid set of points for retriangulation");
                var triangle = new C2DTriangle(node1, node2, P);
                node1.triangles.Add(triangle);
                node2.triangles.Add(triangle);
                P.triangles.Add(triangle);
                result.Add(triangle);
            }
        }

    }

    public class TriangleConstruction {
        public ChewPoint node1 {get; set;}
        public ChewPoint node2 { get; set; }
        public ChewPoint node3 { get; set; }
    }

   

   
}
