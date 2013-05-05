using Algorithms;
using GeoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private int maxSize = 500;
        GaSPointEdgeSet result;
        List<C2DPoint> points;

        public Form1()
        {
            InitializeComponent();
            result = new GaSPointEdgeSet();
            testGuibasAndStolfi(10);
        }

        private void testGuibasAndStolfi(int n)
        {
            Random r = new Random();
            points = new List<C2DPoint>(n);
            for (int i = 0; i < n; i++)
            {
                points.Add(new C2DPoint(r.Next(maxSize), r.Next(maxSize)));
            }
            GuibasAndStolfi GaS =  new GuibasAndStolfi(points);
            result = GaS.Run();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            var blackPen = new Pen(Color.Black, 2);
            foreach (C2DPoint p in points)
            {
                e.Graphics.FillRectangle(Brushes.Red,(float) p.x,(float) p.y, 5f, 5f);
            }
            // var poly = AlgorithmsUtil.RandomConvexPolygon(7, 7, 500);
            var geoDraw = new CGeoDraw();
            foreach (C2DLine edge in result.edgeList)
            {
                geoDraw.Draw(edge, e.Graphics, blackPen);
            }
        }

        private void ShiftButton_Click(object sender, EventArgs e)
        {
            foreach (C2DPoint p in points)
            {
                p.y = maxSize - p.y;
            }
            foreach (C2DLine l in result.edgeList)
            {
                l.SetPointFrom(new C2DPoint(l.GetPointFrom().x, maxSize - l.GetPointFrom().y));
                l.SetPointTo(new C2DPoint(l.GetPointTo().x, maxSize - l.GetPointTo().y));
            }
            pictureBox1.Refresh();
        }
    }
}
