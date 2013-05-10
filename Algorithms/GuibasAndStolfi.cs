﻿using GeoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class GuibasAndStolfi
    {
        private List<C2DPoint> pointList;

        public GuibasAndStolfi()
        {
            pointList = new List<C2DPoint>();
        }

        public GuibasAndStolfi(List<C2DPoint> pList)
        {
            pointList = pList;
        }

        public void SetPoints(List<C2DPoint> pList)
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

            // Create the first base edge
            List<GaSPoint> leftBottomList = leftEdgeSet.GetList();
            List<GaSPoint> rightBottomList = rightEdgeSet.GetList();
            GaSPoint leftBottom = leftEdgeSet.GetLowestPoint();
            GaSPoint rightBottom = rightEdgeSet.GetLowestPoint();

            for (int i = 0; i < leftBottomList.Count; i++)
            {
                C2DLine test = new C2DLine(leftBottom, rightBottom);
                if (test.IsOnRight(leftBottomList[i]))
                {
                    leftBottom = leftBottomList[i];
                }
            }

            for (int i = 0; i < rightBottomList.Count; i++)
            {
                C2DLine test = new C2DLine(leftBottom, rightBottom);
                if (test.IsOnRight(rightBottomList[i]))
                {
                    rightBottom = rightBottomList[i];
                }
            }

            /* // Dit lijkt nergens nodig voor te zijn.. In principe logisch.
            
            int li = 1;
            int ri = 1;
            bool crossesLeft;
            bool crossesRight;
               do
                {
                    C2DLine test = new C2DLine(leftBottom, rightBottom);
                    crossesLeft = false;
                    foreach (C2DLine l in leftEdgeSet.edgeList)
                    {
                        if (test.Crosses(l))
                            crossesLeft = true;
                    }
                    crossesRight = false;
                    foreach (C2DLine l in rightEdgeSet.edgeList)
                    {
                        if (test.Crosses(l))
                            crossesRight = true;
                    }
                    if (crossesLeft)
                    {
                        leftBottom = leftBottomList[li]; li++;
                    }
                    if (crossesRight)
                    {
                        rightBottom = rightBottomList[ri]; ri++;
                    }
                } while (crossesLeft || crossesRight);
            */

            // Merge the two sets
            GaSPointEdgeSet mergedEdges = new GaSPointEdgeSet(leftEdgeSet, rightEdgeSet);

            
            while (true)
            {
                C2DLine baseEdge = new C2DLine(leftBottom, rightBottom);

                List<C2DPoint> leftSortedAngleList = leftBottom.GetSortedAngleList(leftBottom, rightBottom, true);
                List<C2DPoint> rightSortedAngleList = rightBottom.GetSortedAngleList(leftBottom, rightBottom, false);

                C2DPoint leftOption = PotentialPoint(leftSortedAngleList, leftBottom, rightBottom, true, ref mergedEdges);
                C2DPoint rightOption = PotentialPoint(rightSortedAngleList, rightBottom, leftBottom, false, ref mergedEdges);

                // Add after the potential points are found, because otherwise these are added as well
                leftBottom.AddPoint(rightBottom);
                rightBottom.AddPoint(leftBottom);
                mergedEdges.AddEdge(baseEdge); 

                if (leftOption != null && rightOption == null)
                {
                    leftBottom = leftEdgeSet.Get(leftOption); // Only the left option is viable
                }
                else if (leftOption == null && rightOption != null)
                {
                    rightBottom = rightEdgeSet.Get(rightOption); // Only the right option is viable
                }
                else if (leftOption != null && rightOption != null)
                {
                    if (WithinCircumCircle(leftBottom, rightBottom, leftOption, rightOption))
                    {
                        rightBottom = rightEdgeSet.Get(rightOption); // Only the right option is viable
                    }
                    else
                    {
                        leftBottom = leftEdgeSet.Get(leftOption); // Only the left option is viable
                    }
                }
                else
                {
                    break;
                }
            }
            return mergedEdges;
        }

        private C2DPoint PotentialPoint(List<C2DPoint> sortedList, C2DPoint start, C2DPoint other, bool left, ref GaSPointEdgeSet edgeSet)
        {
            for (int i = 0; i < sortedList.Count; i++)
            {
                double angle;
                if (left)
                    angle = Angle(start, sortedList[i], other);
                else
                    angle = Angle(start, other, sortedList[i]);
                if (angle > Math.PI || angle == 0.0)
                    return null; // The angle may not be larger than 180 degrees clockwise for the point to be a valid option
                // Determine if the next point lies within the circumcircle
                if (i == sortedList.Count - 1) // No next point
                {
                    return sortedList[i]; // Potential point
                }
                else
                {
                    if (!WithinCircumCircle(sortedList[i], start, other, sortedList[i + 1]))
                    {
                        return sortedList[i]; // Potential point
                    }
                    else
                    {
                        // Remove this edge, since it is no longer wanted
                        bool success = edgeSet.Remove(start, sortedList[i]);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determine the angle between edges AB and AC
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private double Angle(C2DPoint A, C2DPoint B, C2DPoint C)
        {
            C2DVector AB = new C2DVector(A, B);
            C2DVector AC = new C2DVector(A, C);
            double cross = AB.Cross(AC);
            if (cross < 0)
                return AB.AngleBetween(AC);
            else
                return Math.PI * 2 - AB.AngleBetween(AC);
        }

        /// <summary>
        /// Determine whether the circumcircle of the first three points contains the final point
        /// </summary>
        /// <param name="t1">Triangle point 1</param>
        /// <param name="t2">Triangle point 1</param>
        /// <param name="t3">Triangle point 1</param>
        /// <param name="p">Considered point</param>
        /// <returns></returns>
        private bool WithinCircumCircle(C2DPoint t1, C2DPoint t2, C2DPoint t3, C2DPoint p)
        {
            C2DPoint c = C2DTriangle.GetCircumCentre(t1, t2, t3);
            return c.Distance(p) < c.Distance(t1);
        }
    }

    /// <summary>
    /// Sort helper.
    /// </summary>
    public class PointLeftToRightBottomUp : IComparer<C2DPoint>
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
