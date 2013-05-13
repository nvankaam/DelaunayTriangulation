﻿using GeoLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Algorithms
{
    public class Evaluater
    {
        private long gui { get; set; }
        private long chews { get; set; }
        private C2DPolygon chewsPolygon { get; set; }
        private List<Vertex> Vertices { get; set; }
        public void EvaluateLoop(int times)
        {
            var iterations = new List<int>() {500, 1000, 1500, 2000,2500 ,3000};
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
                Vertices = AlgorithmsUtil.RandomConvexPolygonImproved(size, 100 * size);
                var points = Vertices.Select(o => o.Point).ToList();
                //var polygon = AlgorithmsUtil.ConvertToPolygon(vertices);
                //chewsPolygon = polygon;
                Debug.WriteLine("Got the triangles for size: "+size);
                
                var threadStart = new ThreadStart(EvaluateChews);
                //gui += EvaluateGui(points);
                var T = new Thread(threadStart, 1024 * 1024 * 64);
                T.Start();
                T.Join();
                
                
            }
            chews = chews / times;
            gui = gui / times;
            Debug.WriteLine("Guibas size "+size+" took: " + gui);
            Debug.WriteLine("Chews size " + size + " took: " + chews);
        }

        public void EvaluateChews()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            NewChews.RunOnList(Vertices);
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
