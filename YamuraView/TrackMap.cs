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
            int widthOffset = 0;
            int height = chartPanel.Height;
            int heightOffset = 0;
            if (width < height)
            {
                heightOffset = (height - width) / 2;
                height = width;
            }
            if (height < width)
            {
                widthOffset = (width - height) / 2;
                width = height;
            }
            Pen pathPen = new Pen(Color.Red);
            GraphicsPath drawPath = new GraphicsPath();
            drawPath.AddEllipse(widthOffset, heightOffset, width, height);
            drawPath.CloseFigure();
            using (Graphics chartGraphics = chartPanel.CreateGraphics())
            {
                chartGraphics.Clear(chartPanel.BackColor);
                chartGraphics.DrawPath(pathPen, drawPath);
            }
        }
    }
}
