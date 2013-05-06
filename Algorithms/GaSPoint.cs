using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GaSPoint : C2DPoint, IComparable
    {

        public List<C2DPoint> points;

        public GaSPoint(C2DPoint Other) : base(Other) 
        {
            points = new List<C2DPoint>();
        }

        public GaSPoint(int x, int y) : base(x, y) 
        {
            points = new List<C2DPoint>();
        }

        public void AddPoint(C2DPoint point)
        {
            points.Add(point);
        }

        public bool RemovePoint(C2DPoint point)
        {
            return points.Remove(point);
        }

        public void RemoveEdge(int i)
        {
            points.RemoveAt(i);
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

        public List<C2DPoint> GetSortedList(bool reverse)
        {
            PointLeftToRightBottomUp comparer = new PointLeftToRightBottomUp();
            points.Sort(comparer);
            if (reverse)
                points.Reverse();
            return points;
        }

        public int CompareTo(object obj)
        {
            GaSPoint p = obj as GaSPoint;
            if ((object)p == null)
            {
                throw new InvalidCastException();
            }
            if (this.x == p.x)
                if (this.y > p.y)
                    return 1;
                else if (this.y < p.y)
                    return -1;
                else
                    return 0;
            if (this.x > p.x)
                return 1;
            else if (this.x < p.x)
                return -1;
            else
                return 0;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }

        public List<C2DPoint> GetSortedAngleList(C2DPoint a, C2DPoint b, bool left)
        {
            List<C2DPoint> low = new List<C2DPoint>(points);
            low.Sort(new IncreasingCWAngle(a, b, left));
            return low;
        }
    }
}
