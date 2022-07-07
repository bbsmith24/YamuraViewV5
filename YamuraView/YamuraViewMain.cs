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
    public partial class YamuraViewMain : Form
    {
        // DataLogger contains session(s), which contain channel(s) which contain data point(s)
        public static DataLogger dataLogger = new DataLogger();

        public YamuraViewMain()
        {
            InitializeComponent();
            YamuraViewAppContext.appForms.Add(new ManageSessions());
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
        }
        private void addStripChartMenuItem_Click(object sender, EventArgs e)
        {
            YamuraViewAppContext.appForms.Add(new StripChart());
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
        }
        private void addTractionCircleMenuItem_Click(object sender, EventArgs e)
        {
            YamuraViewAppContext.appForms.Add(new TractionCircle());
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
        }
        private void addTrackMapMenuItem_Click(object sender, EventArgs e)
        {
            YamuraViewAppContext.appForms.Add(new TrackMap());
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
        }
        private void addSessionsMenuItem_Click(object sender, EventArgs e)
        {
            YamuraViewAppContext.appForms.Add(new ManageSessions());
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
        }
        private void addDataGridMenuItem_Click(object sender, EventArgs e)
        {
            YamuraViewAppContext.appForms.Add(new DataGrid());
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
        }
        private void OnFormClosed(object sender, EventArgs e)
        {
            // save form size/location in frame?
            //RectangleConverter rectConv = new RectangleConverter();
            //string formpos = rectConv.ConvertToString(YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.IndexOf(sender as Form)]._formPosition);
            //appFormPositions[appForms.IndexOf(sender as SizedForm)] = formpos;
            YamuraViewAppContext.appForms.Remove(sender as Form);
        }
    }
}
