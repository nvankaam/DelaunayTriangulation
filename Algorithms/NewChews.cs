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
    public class NewChews
    {
        /// <summary>
        /// The input polygon for chews algorithm
        /// </summary>
     
        Random RandomGenerator { get; set; }
        GraphManager GM { get; set; }
        public NewChews()
        {
            RandomGenerator = new Random();
            GM = new GraphManager();
        }

        public static List<Vertex> RunOnList(List<Vertex> input)
        {
            var chews = new NewChews();
            chews.GM.Vertices = input;
            return chews.run();
        }
        //public static List<Triangle> RunOnPolygon(List<Vertex> inputVertices) {
        //    var chews = new NewChews() { };
        //    chews.GM.Vertices = inputVertices;
        //    var result = chews.run();
        //    return result;
        //}

        /// <summary>
        /// Runs chews algorithm
        /// </summary>
        public List<Vertex> run()
        {
            var nrOfPoints = GM.Vertices.Count();

            var input = new int[nrOfPoints];
            for (var i = 0; i < nrOfPoints; i++)
            {
                input[i] = i;
            }
            try
            {
                ChewRecur(input.ToList());
            } catch (InvalidOperationException e) {
                Debug.WriteLine("Crashed");
            }
            return GM.Vertices;
        }

        /// <summary>
        /// The recursive method for chew's algorithm
        /// List is chosen above IList because we need foreach, and we only use default Lists anyway
        /// </summary>
        /// <param name="points">List of indices of the points remaining</param>
        /// <returns></returns>
        public void ChewRecur(List<int> remainingPoints)
        {
            if (remainingPoints.Count <= 3)
            {
                if (remainingPoints.Count < 3)
                    throw new InvalidOperationException("Less than two points in the given triangle");
                
                var triangle =  GM.CreateTriangleAndEdges(GM.Vertices[0], GM.Vertices[1], GM.Vertices[2]);
                return;           
            }

            var remainingCount = remainingPoints.Count;
            //Pick a random point (Step 1)
            int randIndex = RandomGenerator.Next(0, remainingPoints.Count - 1);
            //Debug.WriteLine("Random nr: " + randIndex);

            remainingPoints.RemoveAt(randIndex);

            var nodeP = remainingPoints[randIndex];
            var nodeQ = remainingPoints[(randIndex + remainingPoints.Count - 1) % remainingPoints.Count];
            var nodeR = remainingPoints[(randIndex + 1) % remainingPoints.Count];

            ChewRecur(remainingPoints);

            var pqrTriangle = GM.CreateTriangleAndEdges(GM.Vertices[nodeP], GM.Vertices[nodeQ], GM.Vertices[nodeR]);

            //All triangles whose circumcircle contains P.
            var filtered = GetRecursiveConflicitingTriangles(GM.Vertices[nodeP], pqrTriangle, remainingCount, null);

            //Dont need to retriangulate if the only conflicting triangle is pqr
            if(filtered.Count > 1){
                GM.DestroyEdge(GM.CreateOrGet(GM.Vertices[nodeP], GM.Vertices[nodeQ]));
                foreach (var triangle in filtered)
                {
                    if (triangle != pqrTriangle)
                        Retriangulate(triangle, GM.Vertices[nodeP]);
                }
            }

            return;
        }

        /// <summary>
        /// Retriangulate the given triangle with P
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="P"></param>
        private void Retriangulate(Triangle triangle, Vertex P)
        {
            var otherVertex = triangle.GetVertices().Single(o => o != triangle.conflictingEdge.v1 && o != triangle.conflictingEdge.v2);
            GM.DestroyEdge(triangle.conflictingEdge);
            GM.CreateOrGet(otherVertex, P);
        }

        private List<Triangle> GetRecursiveConflicitingTriangles(Vertex P, Triangle currentTriangle, int SearchNr, Edge conflicting)
        {
            var result = new List<Triangle>();
            if(currentTriangle == null)
                return result;
            currentTriangle.conflictingEdge = conflicting;
            var center = currentTriangle.CreateTriangle().GetCircumCentre();
            if (center.Distance(P.Point) <= center.Distance(currentTriangle.Edges[0].v1.Point))
            {
                result.Add(currentTriangle);
                foreach (var edge in currentTriangle.Edges)
                {
                    if (!edge.VisitedBy.ContainsKey(SearchNr) || !edge.VisitedBy[SearchNr])
                    {
                        edge.VisitedBy[SearchNr] = true;
                        result.AddRange(GetRecursiveConflicitingTriangles(P, edge.GetOtherTriangle(currentTriangle), SearchNr, edge));
                    }
                }

            }
            else
            {
                Debug.WriteLine("Nope");
            }
            return result;
        }

        /// <summary>
        /// Retriangulates the set of points with P as endpoint. 
        /// Note this only works for convex polygon points and assumes the points are sorted in the same order as they appear in the polygon!
        /// </summary>
        /// <param name="S"></param>
        /// <param name="selectedPoints"></param>
        /// <param name="P"></param>
        /// <returns></returns>
        //private IEnumerable<ChewTriangle> ReTriangulate(List<int> selectedPoints, int P)
        //{
        //    //Debug.WriteLine("Size of retriangulate set: " + selectedPoints.Count); //Debugging if its actually O(1)
        //    var result = new List<ChewTriangle>();

        //    for (var index = 0; index < selectedPoints.Count; index++)
        //    {
        //        var P1 = selectedPoints[index];
        //        var P2 = selectedPoints[(index+1)%selectedPoints.Count];
        //        if(P1 != P && P2 != P) {
        //            var triangle = new ChewTriangle()
        //            {
        //                P1 = P1,
        //                P2 = P,
        //                P3 = P2,
        //                Triangle = new C2DTriangle(ConvexPoints[P1], ConvexPoints[P], ConvexPoints[P2])
        //            };

        //            //AddTriangle(P1, triangle);
        //            //AddTriangle(P2, triangle);
        //            //AddTriangle(P, triangle);
        //            result.Add(triangle);
        //        }
        //    }
        //    return result;
        //}



    }
}
