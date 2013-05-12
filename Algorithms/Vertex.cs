using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class Vertex
    {
        public C2DPoint Point { get; set; }

        public List<Edge> Edges { get; set; }

        public Vertex()
        {
            Edges = new List<Edge>();
        }

        /// <summary>
        /// Only meant to use for the random convex generation, not in algorithm.
        /// Returns the other edge if the edge list is of size two
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public Edge GetOther(Edge e)
        {
            if (Edges.Count != 2)
                throw new InvalidOperationException("Trying to retrieve other edge from an vertex with more than two edges!");
            return Edges.Single(o => o != e);
        }
    }
}
