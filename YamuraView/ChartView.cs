//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Drawing.Drawing2D;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace YamuraView
//{
//    public class ChartView : System.Windows.Forms.Form
//    {
//        public Rectangle _formPosition;

//        public virtual void InitializeComponent()
//        {
//            this.chartPanel = new System.Windows.Forms.Panel();
//            this.SuspendLayout();
//            // 
//            // chartPanel
//            // 
//            this.chartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.chartPanel.Location = new System.Drawing.Point(0, 0);
//            this.chartPanel.Name = "chartPanel";
//            this.chartPanel.Size = new System.Drawing.Size(622, 363);
//            this.chartPanel.TabIndex = 0;
//            this.chartPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.chartPanel_Paint);
//            // 
//            // ChartView
//            // 
//            this.ClientSize = new System.Drawing.Size(622, 363);
//            this.Controls.Add(this.chartPanel);
//            this.Name = "ChartView";
//            this.ResumeLayout(false);

//        }

//        public System.Windows.Forms.Panel chartPanel;

//        public virtual void chartPanel_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
//        {
//            PaintPanel();
//        }
//        public virtual void PaintPanel()
//        {
//            int width = chartPanel.Width;
//            int height = chartPanel.Height;
//            Pen pathPen = new Pen(Color.Red);
//            GraphicsPath drawPath = new GraphicsPath();
//            drawPath.AddLine(0, 0, width, height);
//            drawPath.CloseFigure();
//            drawPath.AddLine(width, 0, 0, height);
//            using (Graphics chartGraphics = chartPanel.CreateGraphics())
//            {
//                chartGraphics.Clear(Color.White);
//                chartGraphics.DrawPath(pathPen, drawPath);
//            }
//        }
//    }
//}
