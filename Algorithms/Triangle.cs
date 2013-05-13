using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class Triangle
    {
        public List<Edge> Edges { get; set; }
        public Edge conflictingEdge { get; set; }

        public Triangle()
        {
            Edges = new List<Edge>();
        }

        public List<Vertex> GetVertices()
        {
            return Edges.Select(o => o.v1).Concat(Edges.Select(o => o.v2)).Distinct().ToList();
        }

        public C2DTriangle CreateTriangle()
        {
            var points = GetVertices().Select(o => o.Point).ToList();
            return new C2DTriangle(points[0], points[1], points[2]);
        }

        /// <summary>
        /// Returns all triangles adjecent to this one
        /// </summary>
        /// <returns></returns>
        //public List<Triangle> GetAdjecent()
        //{
        //    List<Triangle> result = new List<Triangle>();
        //    Edges.ForEach(o =>
        //    {
        //        var triangle = o.Triangles.Where(t => t != this).SingleOrDefault();
        //        if (triangle != null)
        //            result.Add(triangle);
        //    });
        //    return result;
        //}
    }
}
