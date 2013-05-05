using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GaSPointEdgeSet
    {
        public MySortedList<GaSPoint> pointList { get; set; }
        public List<C2DLine> edgeList { get; set; }

        public GaSPointEdgeSet()
        {
            pointList = new MySortedList<GaSPoint>();
            edgeList = new List<C2DLine>();
        }

        public GaSPointEdgeSet(List<C2DPoint> points)
        {
            pointList = new MySortedList<GaSPoint>();
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

        public GaSPointEdgeSet(GaSPointEdgeSet g1, GaSPointEdgeSet g2)
        {
            pointList = g1.pointList;
            edgeList = g1.edgeList;
            pointList = pointList.Concat(g2.pointList, false);
            edgeList = edgeList.Concat(g2.edgeList).ToList();
        }

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

        public GaSPoint Get(C2DPoint p)
        {
            int i = pointList.IndexOf(new GaSPoint(p));
            if (i == -1)
                return null;
            return new GaSPoint(pointList[i]);
        }

        public List<GaSPoint> GetBottomList()
        {
            List<GaSPoint> low = new List<GaSPoint>(pointList);
            low.Sort(new PointBottumUp());
            return low;
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
    }
}
