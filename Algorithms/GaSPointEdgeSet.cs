using GeoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GaSPointEdgeSet
    {
        public List<GaSPoint> pointList { get; set; }
        public List<C2DLine> edgeList { get; set; }

        public GaSPointEdgeSet()
        {
            pointList = new List<GaSPoint>();
            edgeList = new List<C2DLine>();
        }

        /// <summary>
        /// Add a list of points, and the edges connecting them 
        /// </summary>
        /// <param name="points"></param>
        public GaSPointEdgeSet(List<C2DPoint> points)
        {
            pointList = new List<GaSPoint>();
            edgeList = new List<C2DLine>();
            for (int i = 0; i < points.Count; i++)
            {
                GaSPoint from = new GaSPoint(points[i]);
                for (int j = i + 1; j < points.Count; j++)
                {
                    GaSPoint to = new GaSPoint(points[j]);
                    C2DLine edge = new C2DLine(from, to);
                    AddEdge(edge);
                    from.AddPoint(to);
                    to.AddPoint(from);
                }
            }
        }

        /// <summary>
        /// Merge to GaSPointEdgeSets
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        public GaSPointEdgeSet(GaSPointEdgeSet g1, GaSPointEdgeSet g2)
        {
            pointList = g1.pointList;
            edgeList = g1.edgeList;
            pointList = pointList.Concat(g2.pointList).ToList();
            edgeList = edgeList.Concat(g2.edgeList).ToList();
        }

        /// <summary>
        /// Add a single edge to the edge list. Then make sure that both points are in the pointList and know about the edge.
        /// </summary>
        /// <param name="edge"></param>
        public void AddEdge(C2DLine edge)
        {
            edgeList.Add(edge);
            GaSPoint pointFrom = new GaSPoint(edge.GetPointFrom());
            GaSPoint pointTo = new GaSPoint(edge.GetPointTo());
            int i1 = pointList.IndexOf(pointFrom);
            int i2 = pointList.IndexOf(pointTo);
            if (i1 != -1)
            {
                pointList[i1].AddPoint(pointTo);
            }
            else
            {
                pointFrom.AddPoint(pointTo);
                pointList.Add(pointFrom);
            }
            if (i2 != -1)
            {
                pointList[i2].AddPoint(pointFrom);
            }
            else
            {
                pointTo.AddPoint(pointFrom);
                pointList.Add(pointTo);
            }
        }

        /// <summary>
        /// Get a single point from the pointList
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public GaSPoint Get(C2DPoint p)
        {
            int i = pointList.IndexOf(new GaSPoint(p));
            if (i == -1)
                return null;
            return pointList[i];
        }

        /// <summary>
        /// Get the pointList sorted by increasing y value
        /// </summary>
        /// <returns></returns>
        public List<GaSPoint> GetBottomList()
        {
            List<GaSPoint> low = new List<GaSPoint>(pointList);
            low.Sort(new PointBottumUp());
            return low;
        }

        /// <summary>
        /// Retrieve a copy of the pointList
        /// </summary>
        /// <returns></returns>
        public List<GaSPoint> GetList()
        {
            return new List<GaSPoint>(pointList);
        }

        /// <summary>
        /// Get the point with the lowest y value
        /// </summary>
        /// <returns></returns>
        public GaSPoint GetLowestPoint()
        {
            GaSPoint x = pointList[0];
            for (int i = 1; i < pointList.Count; i++)
            {
                if (pointList[i].y < x.y)
                {
                    x = pointList[i];
                }
            }
            return new GaSPoint(x);
        }

        /// <summary>
        /// Sort helper.
        /// </summary>
        public class PointBottumUp : IComparer<C2DPoint>
        {
            #region IComparer Members
            /// <summary>
            /// Compare function.
            /// </summary>
            public int Compare(C2DPoint A, C2DPoint B)
            {
                if (A.y > B.y)
                    return 1;
                else if (A.y < B.y)
                    return -1;
                else
                    return 0;
            }
            #endregion
        }

        /// <summary>
        /// Remove an edge, and its references in the pointList
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        internal bool Remove(C2DPoint from, C2DPoint to)
        {
            bool success = false;
            // Remove edges
            for (int i = 0; i < edgeList.Count; i++)
            {
                C2DLine l = edgeList[i];
                if (l.GetPointFrom().x == from.x && l.GetPointFrom().y == from.y && l.GetPointTo().x == to.x && l.GetPointTo().y == to.y ||
                    l.GetPointFrom().x == to.x && l.GetPointFrom().y == to.y && l.GetPointTo().x == from.x && l.GetPointTo().y == from.y)
                {
                    edgeList.RemoveAt(i);
                    i--;
                    success = true;
                }
            }            
            
            // Remove points
            int toi = pointList.IndexOf(new GaSPoint(to));
            int fromi = pointList.IndexOf(new GaSPoint(from));
            if (toi == -1)
            {
                Debug.WriteLine("Something is wrong here" + to);
            }
            else
            {
                success = success && pointList[toi].RemovePoint(new GaSPoint(from)); // Do we need to check if this node has any edges left?
            }
            if (fromi == -1)
            {
                Debug.WriteLine("Something is wrong here" + from);
            }
            else
            {
                success = success && pointList[fromi].RemovePoint(new GaSPoint(to)); // Do we need to check if this node has any edges left?
            }

            return success;
        }

        /// <summary>
        /// To string: [point from, point to]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Points = {");
            foreach (GaSPoint p in pointList)
            {
                sb.Append(p + ", ");
            }
            if (pointList.Count != 0)
                sb.Remove(sb.Length - 2, 2);

            sb.Append("}\r\nEdges = {");
            foreach (C2DLine l in edgeList)
            {
                sb.Append("["+ l.GetPointFrom() + ", " + l.GetPointTo()+"] ");
            }

            return sb.ToString();
        }
    }
}
