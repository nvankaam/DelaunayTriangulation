using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GaSPointEdgeSet
    {
        protected List<GaSPoint> pointList { get; set; }
        protected List<C2DLine> edgeList { get; set; }

        public GaSPointEdgeSet() 
        {
            pointList = new List<GaSPoint>();
            edgeList = new List<C2DLine>();
        }

        public GaSPointEdgeSet(List<C2DPoint> points)
        {
            pointList = new List<GaSPoint>();
            edgeList = new List<C2DLine>();
            for (int i = 0; i < points.Count; i++)
            {
                GaSPoint from = new GaSPoint(points[i]);
                for (int j = i + 1; i < points.Count; i++)
                {
                    GaSPoint to = new GaSPoint(points[j]);
                    C2DLine edge = new C2DLine(from, to);
                    edgeList.Add(edge);
                    from.AddEdge(edge);
                    to.AddEdge(edge);
                }
            }
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
                pointList[i1].AddEdge(edge);
            }
            else
            {
                pointFrom.AddEdge(edge);
                pointList.Add(pointFrom);
            }
            if (i2 != -1)
            {
                pointList[i2].AddEdge(edge);
            }
            else
            {
                pointTo.AddEdge(edge);
                pointList.Add(pointTo);
            }
        }
    }
}
