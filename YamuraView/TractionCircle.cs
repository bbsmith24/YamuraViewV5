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
    public partial class TractionCircle : Form
    {
        public TractionCircle()
        {
            InitializeComponent();
            this.Text = "Traction Circle";
        }

        private void chartPanel_Paint(object sender, PaintEventArgs e)
        {
            int width = chartPanel.Width;
            int height = chartPanel.Height;
            int fullWidth = width;
            int fullHeight = height;
            int widthOffset = 0;
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
            Pen pathPen = new Pen(Color.Gray);
            GraphicsPath drawPath = new GraphicsPath();
            drawPath.AddEllipse(widthOffset + 5, heightOffset + 5, width - 10, height - 10);
            drawPath.CloseFigure();
            drawPath.AddLine(fullWidth / 2, 0, fullWidth / 2 , fullHeight);
            drawPath.CloseFigure();
            drawPath.AddLine(0, fullHeight / 2, fullWidth, fullHeight / 2);
            drawPath.CloseFigure();
            //           drawPath.AddLine(width, 0, 0, height);
            using (Graphics chartGraphics = chartPanel.CreateGraphics())
            {
                chartGraphics.Clear(chartPanel.BackColor);
                chartGraphics.DrawPath(pathPen, drawPath);
            }
        }

        private void chartPanel_SizeChanged(object sender, EventArgs e)
        {
            chartPanel.Invalidate();
        }
    }
}
