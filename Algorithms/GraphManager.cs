using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GraphManager
    {
        public List<Vertex> Vertices { get; set; }
        //public List<Edge> Edges { get; set; }
        //public List<Triangle> Triangles { get; set; }

        public GraphManager()
        {
            Vertices = new List<Vertex>();
            //Triangles = new List<Triangle>();
        }

        public void AddVertexAt(Vertex vertex, int index) {
            Vertices.Insert(index, vertex);
        }
        public void AddVertex(Vertex vertex) {
            Vertices.Add(vertex);
        }

        public Edge CreateEdge(Vertex v1, Vertex v2)
        {
            Edge result = new Edge() { v1 = v1, v2 = v2 };
            v1.Edges.Add(result);
            v2.Edges.Add(result);
            return result;
        }

        /// <summary>
        /// Remove all references to the given edge
        /// </summary>
        /// <param name="e"></param>
        public void DestroyEdge(Edge e)
        {
            //if (Triangles.Count != 0)
            //    throw new InvalidOperationException("Trying to destroy an edge that still has a triangle");
            e.v1.Edges.Remove(e);
            e.v2.Edges.Remove(e);
            
        }

    }
}
