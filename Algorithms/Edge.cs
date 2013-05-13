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
        public Dictionary<int, bool> VisitedBy;

        public Edge()
        {
            VisitedBy = new Dictionary<int, bool>();
        }


        /// <summary>
        /// Returns the other triangle than the given triangle on the graph
        /// Returns null if no such triangle could be found.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Triangle GetOtherTriangle(Triangle t)
        {
            var otherVertexOfTriangle = t.GetVertices().Single(o => o != v1 && o != v2);
            var triangleMembers = GetTriangleMembers();
            if (triangleMembers.Count() > 2)
                throw new InvalidOperationException("Edge with more than two triangles?");
            var otherTriangleMember = triangleMembers.SingleOrDefault(o => o != otherVertexOfTriangle);
            if (otherTriangleMember == null)
                return null;
            var edges = new List<Edge>() { this };
            otherTriangleMember.Edges.ForEach(o =>
            {
                if ((o.v1 == v1 || o.v1 == v2 || o.v2 == v1 || o.v2 == v2))
                    edges.Add(o);
            });
            if (edges.Count != 3)
                throw new InvalidOperationException("Triangle with no 3 edges");
            var triangle = new Triangle() { Edges = edges};
            return triangle;
        }

        /// <summary>
        /// Returns the list of vertices that are on a triangle together with the current edge (can be 2 on max)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vertex> GetTriangleMembers()
        {
            var V1Vertices = new List<Vertex>();
            v1.Edges.ForEach(o =>
            {
                if(o.v1 == v2)
                    V1Vertices.Add(o.v2);
                else
                    V1Vertices.Add(o.v1);
            });
            var V2Vertices = new List<Vertex>();
            v2.Edges.ForEach(o =>
            {
                if (o.v1 == v1)
                    V2Vertices.Add(o.v2);
                else
                    V2Vertices.Add(o.v1);
            });
            return V1Vertices.Intersect(V2Vertices);
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
