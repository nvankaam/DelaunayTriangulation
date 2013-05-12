using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class Edge
    {
        public Vertex v1 { get; set; }
        public Vertex v2 { get; set; }
        public List<Triangle> Triangles { get; set; }

        public Edge()
        {
            Triangles = new List<Triangle>();
        }

        /// <summary>
        /// Creates a line from the edge
        /// </summary>
        /// <returns></returns>
        public C2DLine CreateLine()
        {
            return new C2DLine(v1.Point, v2.Point);
        }
    }
}
