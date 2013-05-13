using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    /// <summary>
    /// Class monitoring the graph and updating all references between graph components (edges, vertices, triangles)
    /// </summary>
    public class GraphManager
    {
        public List<Vertex> Vertices { get; set; }

        public GraphManager()
        {
            Vertices = new List<Vertex>();
            //Triangles = new List<Triangle>();
        }

        /// <summary>
        /// Removes all edges from the graph. 
        /// Just used for the cleaning up the graph after using edges to create it
        /// </summary>
        public void RemoveAllEdges()
        {
            foreach (var v in Vertices)
            {
                var arr = v.Edges.ToArray();
                foreach (var e in arr)
                {
                    DestroyEdge(e);
                }
            }
        }

        /// <summary>
        /// Creates a new triangle on the given vertices
        /// Creades the edges if needed
        /// Updates references on existing edges
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <returns></returns>
        public Triangle CreateTriangleAndEdges(Vertex v1, Vertex v2, Vertex v3)
        {
            var triangle = new Triangle();
            var edge1 = CreateOrGet(v1, v2);
            var edge2 = CreateOrGet(v2, v3);
            var edge3 = CreateOrGet(v3, v1);
            triangle.Edges.Add(edge1);
            triangle.Edges.Add(edge2);
            triangle.Edges.Add(edge3);
            return triangle;
        }

   

        /// <summary>
        /// Inserts a vertex into the list at a specific location
        /// </summary>
        /// <param name="vertex"></param>
        /// <param name="index"></param>
        public void AddVertexAt(Vertex vertex, int index) {
            Vertices.Insert(index, vertex);
        }

        /// <summary>
        /// Adds a vertex to the end of the list
        /// </summary>
        /// <param name="vertex"></param>
        public void AddVertex(Vertex vertex) {
            Vertices.Add(vertex);
        }

        /// <summary>
        /// Creates an edge if it does not exist, or retrieves an edge with v1 and v2 as points if it does exist
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public Edge CreateOrGet(Vertex v1, Vertex v2)
        {
            var edge = v1.Edges.FirstOrDefault(o => o.v1 == v2 || o.v2 == v2);
            if (edge == null)
                edge = CreateEdge(v1, v2);
            return edge;
        }

        /// <summary>
        /// Creates an edge
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
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
            e.v1.Edges.Remove(e);
            e.v2.Edges.Remove(e);
            
        }

    }
}
