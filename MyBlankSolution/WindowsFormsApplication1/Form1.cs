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
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var blackPen = new Pen(Color.Black, 2);

            var poly = AlgorithmsUtil.RandomConvexPolygon(7, 7, 500);
            var geoDraw = new CGeoDraw();
            geoDraw.Draw(poly, e.Graphics, blackPen);
        }

        
       


    }
}
