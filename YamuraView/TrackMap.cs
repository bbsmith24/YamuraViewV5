using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class TrackMap : Form
    {
        public TrackMap()
        {
            InitializeComponent();
            this.Text = "Track Map";
        }

        private void chartPanel_Paint(object sender, PaintEventArgs e)
        {
            int width = chartPanel.Width;
            int height = chartPanel.Height;
            Pen pathPen = new Pen(Color.Gray);
            GraphicsPath drawPath = new GraphicsPath();
            for (int x = 0; x < width; x += 100)
            {
                drawPath.AddLine(x, 0, x, width);
                drawPath.CloseFigure();
            }
            for (int y = height; y >= 0; y -= 100)
            {
                drawPath.AddLine(0, y, width, y);
                drawPath.CloseFigure();
            }
            using (Graphics chartGraphics = chartPanel.CreateGraphics())
            {
                chartGraphics.Clear(chartPanel.BackColor);
                chartGraphics.DrawPath(pathPen, drawPath);
            }
        }
    }
}
