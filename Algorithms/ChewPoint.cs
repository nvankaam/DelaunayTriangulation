using GeoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Algorithms
{
    public class ChewPoint : C2DPoint
    {
        public IList<C2DTriangle> triangles { get; set; }

        public ChewPoint()
        {
            triangles = new List<C2DTriangle>();   
        }
    }
}
