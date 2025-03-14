﻿using System;
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
        public static List<Color> colors = new List<Color>();

        public YamuraViewMain()
        {
            InitializeComponent();
            YamuraViewAppContext.appForms.Add(new ManageSessions());
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
            YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
            colors.Add(Color.Red);
            colors.Add(Color.Green);
            colors.Add(Color.Blue);
            colors.Add(Color.Yellow);
            colors.Add(Color.Orange);
            colors.Add(Color.Cyan);
            colors.Add(Color.Magenta);
            colors.Add(Color.LightGreen);
            colors.Add(Color.LightBlue);
            colors.Add(Color.LightYellow);
            colors.Add(Color.LightCyan);
            colors.Add(Color.DarkRed);
            colors.Add(Color.DarkGreen);
            colors.Add(Color.DarkBlue);
            colors.Add(Color.DarkOrange);
            colors.Add(Color.DarkCyan);
            colors.Add(Color.Gray);
            colors.Add(Color.LightGray);
            colors.Add(Color.DarkGray);
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
            bool viewFound = false;
            for(int viewIdx = 0; viewIdx < YamuraViewAppContext.appForms.Count; viewIdx++)
            {
                if (YamuraViewAppContext.appForms[viewIdx] is ManageSessions)
                {
                    viewFound = true;
                    YamuraViewAppContext.appForms[viewIdx].BringToFront();
                    break;
                }
            }
            if (!viewFound)
            {
                YamuraViewAppContext.appForms.Add(new ManageSessions());
                YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Closed += new EventHandler(OnFormClosed);
                YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].MdiParent = this;
                YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].BringToFront();
                YamuraViewAppContext.appForms[YamuraViewAppContext.appForms.Count - 1].Show();
            }
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
