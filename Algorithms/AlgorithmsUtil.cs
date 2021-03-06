﻿using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Algorithms
{
    /// <summary>
    /// Class containing some utility methods
    /// </summary>
    public class AlgorithmsUtil
    {
        public static Random RandomGenerator { get; set; }

        /// <summary>
        /// Generate a random vertex within the given boundaries
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <returns></returns>
        public static Vertex RandomVertex(int xMin, int xMax, int yMin, int yMax)
        {
            var x = RandomGenerator.Next(xMin, xMax);
            var y = RandomGenerator.Next(yMin, yMax);
            var vertex = new Vertex() { Point = new C2DPoint(x, y)};
            return vertex;
        }

        /// <summary>
        /// Temporary function.
        /// Returns a C2DPolygon based on the RandomConvexPolygonImproved method
        /// </summary>
        /// <returns></returns>
        public static C2DPolygon ConvertToPolygon(List<Vertex> vertices)
        {
            var points = vertices.Select(o => o.Point).ToList();
            var result = new C2DPolygon(points, false);
            return result;
        }

        /// <summary>
        /// Creates a random convex polygon.
        /// </summary>
        /// <param name="generationSize"></param>
        /// <param name="maxOffset"></param>
        /// <returns></returns>
        public static List<Vertex> RandomConvexPolygon(int generationSize, int maxOffset)
        {
            int offsetSmaller = 4;
            RandomGenerator = new Random();
            var gm = new GraphManager();
            var edges = new List<Edge>();

            //Create two random vertices
            var v1 = RandomVertex(maxOffset / 2, maxOffset / 2 + maxOffset / offsetSmaller, maxOffset / 2, maxOffset / 2 + maxOffset / offsetSmaller);
            var v2 = RandomVertex(maxOffset / 2 - maxOffset / offsetSmaller, maxOffset / 2, maxOffset / 2 - maxOffset / offsetSmaller, maxOffset / 2);
            gm.AddVertex(v1);
            gm.AddVertex(v2);
            edges.Add(gm.CreateEdge(v1, v2));
            edges.Add(gm.CreateEdge(v2, v1));
            
            //Create all new vertices and edges by splitting an edge in two and introducing a new vertex.
            int i = 2;
            while(i < generationSize) {
                double vertexLength = 0;
                int index = 0;
                //Ignore everythin for a smaller offset than 30. Basically a filter to not create points too close to each other
                while (vertexLength < 30)
                {
                    index = RandomGenerator.Next(0, edges.Count - 1);
                    vertexLength = edges[index].CreateLine().GetLength();
                }
          
                var oldEdge = edges[index];
                var count = edges.Count;
                edges.RemoveAt(index); //Remove old edge
                edges.AddRange(SplitConvexEdge(gm, oldEdge, maxOffset/20, (i+1)%count)); //And add new ones
                i++;
            }
            gm.RemoveAllEdges(); //Clean up the graph by removing all edges from the vertices
            return gm.Vertices;
        }

        /// <summary>
        /// Splits an edge on a convex polygon into two edges by adding a vertex on a random position
        /// While keeping the Convex constraint.
        /// </summary>
        /// <param name="gm"></param>
        /// <param name="e"></param>
        /// <param name="maxOffset"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static List<Edge> SplitConvexEdge(GraphManager gm, Edge e, int maxOffset, int index)
        {
            var edgeLine = e.CreateLine();
            List<Edge> result = new List<Edge>();
            //Create random point on the edge
            var offset = Convert.ToDouble(RandomGenerator.Next(0, 100)) / 122 + 0.1;
            var point = edgeLine.GetPointOn(offset);

            //Calculate offsets
            var dx = e.v2.Point.x - e.v1.Point.x;
            var dy = e.v2.Point.y - e.v1.Point.y;

            //Note swapping offsets for X and Y.
            var offsetX = Math.Abs(dx) / (dy);
            var offsetY = Math.Abs(dy) / (dx * -1);

            //Calculate the maximum endpoint for the new node.
            var endPoint = new C2DPoint(offsetX * maxOffset + point.x, offsetY * maxOffset + point.y);
            var newLine = new C2DLine(point, endPoint);

            var otherVertex1 = e.v1;
            var otherVertex2 = e.v2;
            var otherEdge1 = otherVertex1.GetOther(e).CreateLine();
            var otherEdge2 = otherVertex2.GetOther(e).CreateLine();

            //Compare with both  other edges, to check if the new vertex would still result in a convex polygon. 
            //If not, update the maximum offset for the new vertex
            if (otherVertex1.GetOther(e) != otherVertex2.GetOther(e))
            {
                var intersections = new List<C2DPoint>();
                otherEdge1.GrowFromCentre(100000000);//Hack: to check if they intersect. The library does not support rays in this case
                if (newLine.Crosses(otherEdge1, intersections))
                {
                    var newEndpoint = intersections.Single();
                    newLine = new C2DLine(point, newEndpoint);
                }
                intersections = new List<C2DPoint>();
                otherEdge2.GrowFromCentre(100000000); //Hack: to check if they intersect. The library does not support rays in this case
                if (newLine.Crosses(otherEdge2, intersections))
                {
                    var newEndpoint = intersections.Single();
                    newLine = new C2DLine(point, newEndpoint);
                }
            }

            //Select a random point on the line (which is bound)
            var newlineOffset = Convert.ToDouble(RandomGenerator.Next(0, 100)) / 122+0.1;
            var newPoint = newLine.GetPointOn(offset);

            var newVertex = new Vertex() { Point = newPoint };
            //Indexof might be very slow? Note this is just used for the creation of a testvertex
            gm.AddVertexAt(newVertex, gm.Vertices.IndexOf(otherVertex2));
            gm.DestroyEdge(e);//Unregister the old edge
            result.Add(gm.CreateEdge(otherVertex1, newVertex)); //Add the two new ones
            result.Add(gm.CreateEdge(newVertex, otherVertex2)); // :)
            return result;

        }


        
    }
}
