using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    class IncreasingCWAngle : IComparer<C2DPoint>
    {
        private GeoLib.C2DPoint l;
        private GeoLib.C2DPoint r;
        private bool left;

        public IncreasingCWAngle(GeoLib.C2DPoint l, GeoLib.C2DPoint r, bool left)
        {
            this.l = l;
            this.r = r;
            this.left = left;
        }


        public int Compare(C2DPoint x, C2DPoint y)
        {
            double angleX;
            double angleY;
            if (left)
            {
                angleX = Angle(l, x, r);
                angleY = Angle(l, y, r);
            }
            else
            {
                angleX = Angle(r, l, x);
                angleY = Angle(r, l, y);
            }
            if (angleX < angleY)
                return -1;
            else if (angleX > angleY)
                return 1;
            else
                return 0;
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
    }
}
