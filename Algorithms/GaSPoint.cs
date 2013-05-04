using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GaSPoint : C2DPoint, IComparable
    {

        public GaSPoint(C2DPoint Other) : base(Other) 
        {
            edges = new List<C2DLine>();
        }

        public GaSPoint(int x, int y) : base(x, y) 
        {
            edges = new List<C2DLine>();
        }

        protected List<C2DLine> edges;

        public void AddEdge(C2DLine edge)
        {
            edges.Add(edge);
        }

        public bool RemoveEdge(C2DLine edge)
        {
            return edges.Remove(edge);
        }

        public void RemoveEdge(int i)
        {
            edges.RemoveAt(i);
        }

        public override bool Equals(Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            GaSPoint p = obj as GaSPoint;
            if ((object)p == null)
            {
                return false;
            }
            if (p.x == this.x && p.y == this.y)
            {
                return true;
            }
            return false;
        }
    }
}
