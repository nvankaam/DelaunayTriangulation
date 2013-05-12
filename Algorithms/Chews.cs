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
        /// List of all points on the input
        /// </summary>
        private C2DPoint[] ConvexPoints;

        /// <summary>
        /// Dictionary containing the Triangles a point is currently bound to
        /// </summary>
        //private IDictionary<int, List<ChewTriangle>> PointTriangles = new Dictionary<int, List<ChewTriangle>>();


        ////Ads a triangle to a point;
        //private void AddTriangle(int pointIndex, ChewTriangle triangle)
        //{
        //    if (!PointTriangles.Keys.Contains(pointIndex))
        //    {
        //        PointTriangles[pointIndex] = new List<ChewTriangle>();
        //    }
        //    PointTriangles[pointIndex].Add(triangle);
        //}
        ///// <summary>
        ///// Removes a triangle from a point
        ///// </summary>
        ///// <param name="pointIndex"></param>
        ///// <param name="triangle"></param>
        //private void RemoveTriangle(int pointIndex, ChewTriangle triangle)
        //{
        //    PointTriangles[pointIndex].Remove(triangle);
        //}

        //private List<ChewTriangle> GetTriangles(int pointIndex)
        //{
        //    return PointTriangles[pointIndex];
        //}

        /// <summary>
        /// TODO: Optimize this with the hashtable possible by C2DTriangleComparator
        /// TODO: Not used
        /// </summary>
        /// <returns></returns>
        private bool ListContains(ChewTriangle triangle, List<ChewTriangle> list)
        {
            var comp = new ChewTriangleComparator();
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

        public static List<ChewTriangle> TestRun(int size)
        {
            var convex = AlgorithmsUtil.RndCvxPolygonNewConverted(size, 200);
            //var convex = AlgorithmsUtil.RandomConvexPolygon(size, size);
            var chews = new Chews() { InputPolygon = convex };
            var result = chews.run();
            return result;
        }

        /// <summary>
        /// Runs chews algorithm
        /// </summary>
        public List<ChewTriangle> run()
        {
            if (!InputPolygon.IsConvex())
                throw new InvalidOperationException("Chews algorithm was called on a non-convex polygon");

            var points = new List<C2DPoint>();
            InputPolygon.GetPointsCopy(points);
            ConvexPoints = points.ToArray();
            var nrOfPoints = ConvexPoints.Count();

            var input = new int[nrOfPoints];
            for (var i = 0; i < nrOfPoints; i++)
            {
                input[i] = i;
            }
            return ChewRecur(input.ToList());
        }

        /// <summary>
        /// The recursive method for chew's algorithm
        /// List is chosen above IList because we need foreach, and we only use default Lists anyway
        /// </summary>
        /// <param name="points">List of indices of the points remaining</param>
        /// <returns></returns>
        public List<ChewTriangle> ChewRecur(List<int> remainingPoints)
        {
            if (remainingPoints.Count <= 3)
            {
                if (remainingPoints.Count < 3)
                    throw new InvalidOperationException("Less than two points in the given triangle");
                var triangle = new ChewTriangle() {
                    /**
                    P1 =remainingPoints[0], 
                    P2 = remainingPoints[1], 
                    P3 = remainingPoints[2], **/
                    Triangle = new C2DTriangle(ConvexPoints[remainingPoints[0]], ConvexPoints[remainingPoints[1]], ConvexPoints[remainingPoints[2]])
                };
                /**
                AddTriangle(remainingPoints[0], triangle);
                AddTriangle(remainingPoints[1], triangle);
                AddTriangle(remainingPoints[2], triangle);
                **/
                var singleTriangle = new List<ChewTriangle>();
                singleTriangle.Add(triangle);
                return singleTriangle;
            }

            //Pick a random point (Step 1)
            int randIndex = RandomGenerator.Next(0, remainingPoints.Count - 1);
            Debug.WriteLine("Random nr: " + randIndex);

            remainingPoints.RemoveAt(randIndex);

            var nodeP = remainingPoints[randIndex];
            var nodeQ = remainingPoints[(randIndex + remainingPoints.Count - 1) % remainingPoints.Count];
            var nodeR = remainingPoints[(randIndex + 1) % remainingPoints.Count];

            List<ChewTriangle> triangles = ChewRecur(remainingPoints);
            

            var pqrTriangle = new ChewTriangle() {
                P1 = nodeP,
                P2 = nodeQ,
                P3 = nodeR,
                Triangle = new C2DTriangle(ConvexPoints[nodeP], ConvexPoints[nodeQ], ConvexPoints[nodeR])
            };
            triangles.Add(pqrTriangle);

            //Store reference to the triangle on the points, for later use
            //AddTriangle(nodeP, pqrTriangle);
            //AddTriangle(nodeQ, pqrTriangle);
            //AddTriangle(nodeR, pqrTriangle);
            

            //All triangles whose circumcircle contains P.
            //TODO: Optimize this so it is in O(n)
            //TODO: Don't check the pqr triangle because we already know its within the range of P.
            var filtered = triangles.Where(o => {
                    var center = o.Triangle.GetCircumCentre();
                    return center.Distance(ConvexPoints[nodeP]) <= center.Distance(ConvexPoints[o.P1]);
                }
            ).ToList();

          //  filtered.Add(pqrTriangle);

            if (filtered.Count() == 0)
                return triangles;

            var filteredPoints = new List<int>();
            foreach(var triangle in filtered) {
                filteredPoints.Add(triangle.P1);
                filteredPoints.Add(triangle.P2);
                filteredPoints.Add(triangle.P3);
            }
            //Remove doubles
            filteredPoints = filteredPoints.Distinct().ToList();

            filteredPoints.Sort();

            //Obtain all points
            

            //Remove the filtered list from the list
            filtered.ForEach(o =>
            {
                //RemoveTriangle(o.P1, o);
                //RemoveTriangle(o.P2, o);
                //RemoveTriangle(o.P3, o);
                triangles.Remove(o);
            });
            

            //Add the retriangulated version back to the list
            triangles.AddRange(ReTriangulate(filteredPoints, nodeP));

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
        private IEnumerable<ChewTriangle> ReTriangulate(List<int> selectedPoints, int P)
        {
            Debug.WriteLine("Size of retriangulate set: " + selectedPoints.Count); //Debugging if its actually O(1)
            var result = new List<ChewTriangle>();

            for (var index = 0; index < selectedPoints.Count; index++)
            {
                var P1 = selectedPoints[index];
                var P2 = selectedPoints[(index+1)%selectedPoints.Count];
                if(P1 != P && P2 != P) {
                    var triangle = new ChewTriangle()
                    {
                        P1 = P1,
                        P2 = P,
                        P3 = P2,
                        Triangle = new C2DTriangle(ConvexPoints[P1], ConvexPoints[P], ConvexPoints[P2])
                    };

                    //AddTriangle(P1, triangle);
                    //AddTriangle(P2, triangle);
                    //AddTriangle(P, triangle);
                    result.Add(triangle);
                }
            }
            return result;
        }



    }
}
