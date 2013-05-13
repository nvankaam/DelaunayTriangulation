using GeoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Algorithms
{
    /// <summary>
    /// Hacking class used to create the testdata
    /// Not interesting for readers
    /// </summary>
    public class Evaluater
    {
        private long gui { get; set; }
        private long chews { get; set; }
        private C2DPolygon chewsPolygon { get; set; }
        private List<Vertex> Vertices { get; set; }
        public void EvaluateLoop(int times)
        {
            var iterations = new List<int>() { 1000,2000,3000,4000,5000,6000,7000,8000,9000};
            foreach (var size in iterations)
            {
                Evaluate(size, times);
            }
        }



        public void Evaluate(int size, int times)
        {
            gui = 0;
            chews = 0;
            for (int i = 0; i < times; i++)
            {
                Vertices = AlgorithmsUtil.RandomConvexPolygon(size, 10000 * size);
                var points = GenerateRandomNonConvex(size, 100*size);
                //var polygon = AlgorithmsUtil.ConvertToPolygon(vertices);
                //chewsPolygon = polygon;
                //Debug.WriteLine("Got the triangles for size: "+size);
                
                
                EvaluateGui(points);
                var threadStart = new ThreadStart(EvaluateChews);
                var T = new Thread(threadStart, 1024 * 1024 * 64);
                T.Start();
                
                T.Join();
                
                
            }
            chews = chews / times;
            gui = gui / times;
            Debug.WriteLine("Guibas size "+size+" took: " + gui);
            Debug.WriteLine("Chews size " + size + " took: " + chews);
        }

        public static List<C2DPoint> GenerateRandomNonConvex(int n, int maxSize)
        {
            Random r = new Random();
            var points = new List<C2DPoint>(n);
            for (int i = 0; i < n; i++)
            {
                points.Add(new C2DPoint(r.Next(maxSize), r.Next(maxSize)));
            }
            GuibasAndStolfi GaS = new GuibasAndStolfi(points);
            return points;
        }

        public void EvaluateChews()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Chews.RunOnList(Vertices);
            stopWatch.Stop();
            chews += stopWatch.ElapsedMilliseconds;
        }

        public void EvaluateGui(List<C2DPoint> points)
        {
            var guiObject = new GuibasAndStolfi();
            guiObject.SetPoints(points);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            guiObject.Run();
            stopWatch.Stop();
            gui += stopWatch.ElapsedMilliseconds;
        }
    }
}
