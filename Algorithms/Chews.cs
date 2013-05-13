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
        Random RandomGenerator { get; set; }
        GraphManager GM { get; set; }
        public Chews()
        {
            RandomGenerator = new Random();
            GM = new GraphManager();
        }

        /// <summary>
        /// Static method to bootstrap the process.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<Vertex> RunOnList(List<Vertex> input)
        {
            var chews = new Chews();
            chews.GM.Vertices = input;
            return chews.run();
        }


        /// <summary>
        /// Runs chews algorithm, and returns all vertices with the references to the edges
        /// </summary>
        public List<Vertex> run()
        {
            var nrOfPoints = GM.Vertices.Count();

            var input = new int[nrOfPoints];
            for (var i = 0; i < nrOfPoints; i++)
            {
                input[i] = i;
            }
           
            ChewRecur(input.ToList());
  
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

            remainingPoints.RemoveAt(randIndex);
            var nodeP = remainingPoints[randIndex];
            var nodeQ = remainingPoints[(randIndex + remainingPoints.Count - 1) % remainingPoints.Count];
            var nodeR = remainingPoints[(randIndex + 1) % remainingPoints.Count];

            //Recursively call self, obtaining the Delaunauy graph for every vertex except P.
            ChewRecur(remainingPoints);

            var pqrTriangle = GM.CreateTriangleAndEdges(GM.Vertices[nodeP], GM.Vertices[nodeQ], GM.Vertices[nodeR]);

            //All triangles whose circumcircle contains P.
            var filtered = GetRecursiveConflicitingTriangles(GM.Vertices[nodeP], pqrTriangle, remainingCount, null);

            //Dont need to retriangulate if the only conflicting triangle is pqr
            if(filtered.Count > 1){

                //Retriangulate all conflicting triangles
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
        /// Basically removes the conflicting edge from the graph and introduces a new one from the vertex that was not on the removed edge and P
        /// </summary>
        /// <param name="triangle"></param>
        /// <param name="P"></param>
        private void Retriangulate(Triangle triangle, Vertex P)
        {
            var otherVertex = triangle.GetVertices().Single(o => o != triangle.conflictingEdge.v1 && o != triangle.conflictingEdge.v2);
            GM.DestroyEdge(triangle.conflictingEdge);
            GM.CreateOrGet(otherVertex, P);
        }

        /// <summary>
        /// Recursively retrieves all conflicting triangles with P based on their circumcircle
        /// This method should be O(S) where S is the number of conflicting triangles, which is constant for a convex polygon
        /// However due to a bug this seems to be untrue.
        /// </summary>
        /// <param name="P"></param>
        /// <param name="currentTriangle"></param>
        /// <param name="SearchNr"></param>
        /// <param name="conflicting"></param>
        /// <returns></returns>
        private List<Triangle> GetRecursiveConflicitingTriangles(Vertex P, Triangle currentTriangle, int SearchNr, Edge conflicting)
        {
            var result = new List<Triangle>();
            if(currentTriangle == null)
                return result;
            currentTriangle.conflictingEdge = conflicting;
            var center = currentTriangle.CreateTriangle().GetCircumCentre();
            if (center.Distance(P.Point) <= center.Distance(currentTriangle.Edges[0].v1.Point))
            {
                //If the circumcircle contains P, add the triangle to the list
                result.Add(currentTriangle);
                //And continue over all triangles adjecent to the current triangles
                foreach (var edge in currentTriangle.Edges)
                {
                    //If the edge was not visited yet, continue
                    if (!edge.VisitedBy.ContainsKey(SearchNr) || !edge.VisitedBy[SearchNr])
                    {
                        edge.VisitedBy[SearchNr] = true;
                        result.AddRange(GetRecursiveConflicitingTriangles(P, edge.GetOtherTriangle(currentTriangle), SearchNr, edge));
                    }
                }

            }
            return result;
        }

    }
}
