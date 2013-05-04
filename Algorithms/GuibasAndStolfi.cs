using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GuibasAndStolfi
    {
        private List<C2DPoint> pointList;

        public GuibasAndStolfi(List<C2DPoint> pList)
        {
            pointList = pList;
        }

        public GaSPointEdgeSet Run()
        {
            List<C2DPoint> sortedList = new List<C2DPoint>(pointList);

            // Make sure that the list is sorted first. From low to high x values. If equal, from low to high y values.
            PointLeftToRightBottomUp comparer = new PointLeftToRightBottomUp();
            sortedList.Sort(comparer);

            return GaSRecur(sortedList);
        }

        private GaSPointEdgeSet GaSRecur(List<C2DPoint> points)
        {
            // If the number of points in the list is smaller or equal to three, return the edges between them. Edges are added such that the first point is the leftmost
            if (points.Count <= 3)
            {
                return new GaSPointEdgeSet(points);
            }

            // Split the nodes in left and right sets
            GaSPointEdgeSet leftEdgeSet = GaSRecur(new List<C2DPoint>(points.Take(points.Count / 2)));
            GaSPointEdgeSet rightEdgeSet = GaSRecur(new List<C2DPoint>(points.Skip(points.Count / 2)));

            GaSPointEdgeSet mergedEdges = new GaSPointEdgeSet();

            // Merge the two sets
            while (true)
            {
                C2DPoint leftBottom = LowestPoint(leftEdgeSet);
                C2DPoint rightBottom = LowestPoint(rightEdgeSet);
                C2DLine baseEdge = new C2DLine(leftBottom, rightBottom);

            }

            return mergedEdges;
        }

        private C2DPoint LowestPoint(GaSPointEdgeSet edges)
        {
            C2DPoint lowest = new C2DPoint(0, Int32.MaxValue);
            foreach (C2DLine l in edges)
            {
                if (l.GetPointFrom().y < lowest.y)
                {
                    lowest = l.GetPointFrom();
                }
                if (l.GetPointTo().y < lowest.y)
                {
                    lowest = l.GetPointTo();
                }
            }
            return lowest;
        }


    }



    /// <summary>
    /// Sort helper.
    /// </summary>
    public class PointLeftToRightBottomUp : IComparer< C2DPoint>
    {
        #region IComparer Members
        /// <summary>
        /// Compare function.
        /// </summary>
        public int Compare(C2DPoint A, C2DPoint B)
        {
            if (A.x == B.x)
                if (A.y > B.y)
                    return 1;
                else if (A.y < B.y)
                    return -1;
                else
                    return 0;
            if (A.x > B.x)
                return 1;
            else if (A.x < B.x)
                return -1;
            else
                return 0;
        }
        #endregion
    }
}
