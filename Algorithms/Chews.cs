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
        private C2DPoint[] ConvexPoints;
        private IDictionary<C2DPoint, List<C2DTriangle>> PointTriangles = new Dictionary<C2DPoint, List<C2DTriangle>>(new C2DPointComparator());



        //Ads a triangle to a point;
        private void AddTriangle(C2DPoint point, C2DTriangle triangle)
        {
            if (!PointTriangles.Keys.Contains(point))
            {
                PointTriangles[point] = new List<C2DTriangle>();
            }
            PointTriangles[point].Add(triangle);
        }
        /// <summary>
        /// Removes a triangle from a point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="triangle"></param>
        private void RemoveTriangle(C2DPoint point, C2DTriangle triangle)
        {
            PointTriangles[point].Remove(triangle);
        }

        private List<C2DTriangle> GetTriangles(C2DPoint point)
        {
            return PointTriangles[point];
        }

        /// <summary>
        /// TODO: Optimize this with the hashtable possible by C2DTriangleComparator
        /// </summary>
        /// <returns></returns>
        private bool ListContains(C2DTriangle triangle, List<C2DTriangle> list)
        {
            var comp = new C2DTriangleComparator();
            return list.Any(o =>comp.Equals(triangle, o));
        }

        /// <summary>
        /// The input polygon for chews algorithm
        /// </summary>
        public C2DPolygon InputPolygon {get; set;}
        Random RandomGenerator { get; set; }

        public Chews() {
            RandomGenerator = new Random();
        }

        public static List<C2DTriangle> TestRun(int size)
        {
            var convex = AlgorithmsUtil.RandomConvexPolygon(size, size);
            var chews = new Chews() { InputPolygon = convex };
            var result = chews.run();
            return result;
        }

        /// <summary>
        /// Runs chews algorithm
        /// </summary>
        public List<C2DTriangle> run()
        {
            if (!InputPolygon.IsConvex())
                throw new InvalidOperationException("Chews algorithm was called on a non-convex polygon");

            var points = new List<C2DPoint>();
            InputPolygon.GetPointsCopy(points);
            return ChewRecur(points);

        }

        /// <summary>
        /// The recursive method for chew's algorithm
        /// List is chosen above IList because we need foreach, and we only use default Lists anyway
        /// </summary>
        /// <param name="points">The points in the diagram.</param>
        /// <returns></returns>
        public List<C2DTriangle> ChewRecur(List<C2DPoint> points)
        {
            if (points.Count <= 3)
            {
                if (points.Count < 3)
                    throw new InvalidOperationException("Less than two points in the given triangle");
                var triangle = new C2DTriangle(points[0], points[1], points[2]);
                AddTriangle(points[0], triangle);
                AddTriangle(points[1], triangle);
                AddTriangle(points[2], triangle);

                var singleTriangle = new List<C2DTriangle>();
                singleTriangle.Add(triangle);
                return singleTriangle;
            }

            //Pick a random point (Step 1)
            int randIndex = RandomGenerator.Next(0, points.Count);
            Debug.WriteLine("Random nr: " + randIndex);

            points.RemoveAt(randIndex);

            var nodeP = points[randIndex];
            var nodeQ = points[(randIndex + points.Count - 1) % points.Count];
            var nodeR = points[(randIndex + 1) % points.Count];

            List<C2DTriangle> triangles = ChewRecur(points);
            

            var pqrTriangle = new C2DTriangle(nodeP, nodeQ, nodeR);
            triangles.Add(pqrTriangle);

            //Store reference to the triangle on the points, for later use
            AddTriangle(nodeP, pqrTriangle);
            AddTriangle(nodeQ, pqrTriangle);
            AddTriangle(nodeR, pqrTriangle);
            

            //All triangles whose circumcircle contains P.
            //TODO: Optimize this so it is in O(n)
            //TODO: Don't check the pqr triangle because we already know its within the range of P.
            var filtered = triangles.Where(o => {
                    var center = o.GetCircumCentre();
                    return center.Distance(nodeP) <= center.Distance(o.p1);
                }
            ).ToList();

            if (filtered.Count() == 0)
                return triangles;


            //Obtain all points
            var filteredPoints = points.Where(o =>
            {
                var selTriangles = GetTriangles(o);
                foreach (var t in selTriangles)
                {
                    if (ListContains(t, filtered))
                        return true;
                }
                return false;
            }).ToList();

            //Remove the filtered list from the list
            filtered.ForEach(o =>
            {
                RemoveTriangle(o.p1, o);
                RemoveTriangle(o.p2, o);
                RemoveTriangle(o.p3, o);
                triangles.Remove(o);
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
        private IEnumerable<C2DTriangle> ReTriangulate(List<C2DTriangle> S, List<C2DPoint> selectedPoints, C2DPoint P)
        {
            Debug.WriteLine("Size of retriangulate set: " + S.Count()); //Debugging if its actually O(1)
            var result = new List<C2DTriangle>();

            //Remove all references to the current triangles
            foreach (var p in selectedPoints)
            {
                GetTriangles(p).RemoveAll(o => ListContains(o, S));
            }

            var pointStack = new Stack<C2DPoint>(selectedPoints);

            var node1 = pointStack.Pop();
            var firstNode = node1;
            var node2 = node1;

            while (pointStack.Count > 0)
            {
                node2 = node1;
                node1 = pointStack.Pop();
               
                var triangle = new C2DTriangle(node1, node2, P);
                AddTriangle(node1, triangle);
                AddTriangle(node2, triangle);
                AddTriangle(P, triangle);
                result.Add(triangle);
            }
            
            //Add last triangle
            var lastTriangle = new C2DTriangle(node1, firstNode, P);
            AddTriangle(node2, lastTriangle);
            AddTriangle(firstNode, lastTriangle);
            AddTriangle(P, lastTriangle);
            result.Add(lastTriangle);


            return result;
        }



    }
}
