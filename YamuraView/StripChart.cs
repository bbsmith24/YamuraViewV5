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
    public partial class StripChart : Form
    {
        bool dragStart = false;
        int dragSession = 0;
        int mouseLast = 0;
        List<Color> colors = new List<Color>();

        List<ChannelDisplayInfo> channels = new List<ChannelDisplayInfo>();
        public StripChart()
        {
            InitializeComponent();
            this.Text = "Strip Chart";
            chartPanel.BringToFront();
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
        private void chartPanel_Paint(object sender, PaintEventArgs e)
        {
            #region nothing to display yet
            if (channels.Count == 0)
            {
                return;
            }
            #endregion
            #region initialize mouse/cursor moves
            //for (int moveIdx = 0; moveIdx < startMouseMove.Count; moveIdx++)
            //{
            //    startMouseMove[moveIdx] = false;
            //    startMouseDrag[moveIdx] = false;
            //    chartStartCursorPos[moveIdx] = new Point(0, 0);
            //    chartLastCursorPos[moveIdx] = new Point(0, 0);
            //}
            #endregion
            #region build unscaled path if needed (initial display, values changed after load, invert, filter etc)
            bool initialValue = true;
            PointF[] points = new PointF[] { new PointF(), new PointF() };
            foreach (ChannelDisplayInfo channel in channels)
            {
                // don't create path (clear any path present) if
                // no data channel/points
                // no axis channel/points
                if ((channel.dataChannel.dataPoints == null) ||
                    (channel.dataChannel.dataPoints.Count == 0) ||
                    (channel.axisChannel == null) ||
                    (channel.axisChannel.dataPoints == null) ||
                    (channel.axisChannel.dataPoints.Count == 0))
                {
                    channel.ChannelPath.ClearMarkers();
                    continue;
                }
                // data and axis present, need to regenerate unscaled path
                if((channel.ChannelPath == null) || 
                   (channel.ChannelPath.PointCount == 0))
                {
                    initialValue = true;
                    foreach (KeyValuePair<float, DataPoint> curData in channel.dataChannel.DataPoints)
                    {
                        // x axis is time - direct lookup
                        if (channel.axisChannel.ChannelName == "Time")
                        {
                            points[1] = new PointF(curData.Key, curData.Value.PointValue);
                        }
                        // x axis is not time - find nearest time in axis channel, 
                        else
                        {
                            DataPoint tst = channel.axisChannel.dataPoints.FirstOrDefault(i => i.Key >= curData.Key).Value;
                            if (tst == null)
                            {
                                continue;
                            }
                            points[1] = new PointF(tst.PointValue, curData.Value.PointValue);
                        }
                        if (initialValue)
                        {
                            initialValue = false;
                            points[0] = new PointF(points[1].X, points[1].Y);
                            continue;
                        }
                        channel.ChannelPath.AddLine(points[0], points[1]);
                        points[0] = new PointF(points[1].X, points[1].Y);
                    }
                }
            }
            #endregion
            #region get max range for X and Y axes
            float[] xAxisRange = new float[2] { float.MaxValue, float.MinValue };
            Dictionary<string, float[]> yAxesRanges = new Dictionary<string, float[]>();
            //float[] yAxisRange = new float[2] { float.MaxValue, float.MinValue };
            foreach (ChannelDisplayInfo channel in channels)
            {
                #region skip if channel is not displayed
                if (!channel.ShowChannel)
                {
                    continue;
                }
                #endregion
                System.Diagnostics.Debug.WriteLine(channel.dataChannel.ChannelName + " (" + channel.SessionIdx + ")");
                System.Diagnostics.Debug.WriteLine("X axis " + channel.axisChannel.DataRange[0] + " to " + channel.axisChannel.DataRange[1]);
                System.Diagnostics.Debug.WriteLine("Y axis " + channel.dataChannel.DataRange[0] + " to " + channel.dataChannel.DataRange[1]);

                xAxisRange[0] = xAxisRange[0] < channel.axisChannel.DataRange[0] ? xAxisRange[0] : channel.axisChannel.DataRange[0];
                xAxisRange[1] = channel.axisChannel.DataRange[1] < xAxisRange[1] ? xAxisRange[1] : channel.axisChannel.DataRange[1];

                if (yAxesRanges.ContainsKey(channel.dataChannel.ChannelName))
                {
                    yAxesRanges[channel.dataChannel.ChannelName][0] = yAxesRanges[channel.dataChannel.ChannelName][0] < channel.dataChannel.DataRange[0] ? yAxesRanges[channel.dataChannel.ChannelName][0] : channel.dataChannel.DataRange[0];
                    yAxesRanges[channel.dataChannel.ChannelName][1] = channel.dataChannel.DataRange[1] < yAxesRanges[channel.dataChannel.ChannelName][1] ? yAxesRanges[channel.dataChannel.ChannelName][1] : channel.dataChannel.DataRange[1];
                }
                else
                {
                    yAxesRanges.Add(channel.dataChannel.ChannelName, new float[2] { channel.dataChannel.DataRange[0], channel.dataChannel.DataRange[1] });
                }
            }
            System.Diagnostics.Debug.WriteLine("Overall");
            System.Diagnostics.Debug.WriteLine("X axis " + xAxisRange[0] + " to " + xAxisRange[1]);
            foreach (KeyValuePair<string, float[]> yAxis in yAxesRanges)
            {
                System.Diagnostics.Debug.WriteLine("Y axis " + yAxis.Key + yAxis.Value[0] + " to " + yAxis.Value[1]);
            }
            #endregion
            #region draw path to display
            Pen pathPen = new Pen(Color.Red);
            int width = chartPanel.Width;
            int height = chartPanel.Height;
            // x and y scale
            float[] displayScale = new float[] { 1.0F, 1.0F };
            // process all channels
            foreach (ChannelDisplayInfo channel in channels)
            {
                #region skip if channel is not displayed
                if (!channel.ShowChannel)
                {
                    continue;
                }
                #endregion
                #region draw to transformed graphic context
                pathPen = new Pen(channel.ChannelColor == null ? Color.Red : channel.ChannelColor);
                using (Graphics chartGraphics = chartPanel.CreateGraphics())
                {
                    Rectangle clipRect = chartPanel.Bounds;
                    clipRect.Inflate(-5, -5);
                    chartGraphics.SetClip(clipRect);

                    //displayScale[0] = (float)clipRect.Width / (channel.axisChannel.DataRange[1] - channel.axisChannel.DataRange[0]);
                    //displayScale[1] = (float)clipRect.Height / (channel.dataChannel.DataRange[1] - channel.dataChannel.DataRange[0]);
                    displayScale[0] = (float)clipRect.Width / (xAxisRange[1] - xAxisRange[0]);
                    displayScale[1] = (float)clipRect.Height / (yAxesRanges[channel.dataChannel.ChannelName][1] - yAxesRanges[channel.dataChannel.ChannelName][0]);
                    displayScale[1] *= -1.0F;
                    // translate to lower left corner of display area
                    //chartGraphics.TranslateTransform(chartBorder, (float)clipRect.Height/* + chartBorder*/);
                    // scale to display range in X and Y
                    chartGraphics.ScaleTransform(displayScale[0], displayScale[1]);
                    // translate by -1 * minimum display range + axis offset (scrolling)
                    chartGraphics.TranslateTransform(0.0F + channel.AxisOffsetX[0],
                        //, //-1 * axisChannel.dataChannel.DataRange[0],
                    //                                    //+ ChartOwner.ChartAxes[xChannelName].AssociatedChannels[curChanInfo.SessionIndex.ToString() + "-" + xChannelName].AxisOffset[0],  // offset X
                                                        -1 * (yAxesRanges[channel.dataChannel.ChannelName][1] - yAxesRanges[channel.dataChannel.ChannelName][0]));// channel.dataChannel.DataRange[1]);// yAxis.Value.AxisDisplayRange[0]);
                    //                                    //+ ChartOwner.ChartAxes[xChannelName].AssociatedChannels[curChanInfo.SessionIndex.ToString() + "-" + xChannelName].AxisOffset[1]);  // offset Y
//                                                                                                                                                                                            // set pen width to 0 (1 pixel regardless of scaling)
                    pathPen.Width = 0;
                    // draw the path
                    chartGraphics.DrawPath(pathPen, channel.ChannelPath);
                    // reset to original orientation
                    chartGraphics.ResetTransform();
                }
                #endregion
            }
            #endregion
        }
        private void chartPanel_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void addChannelsMenuItem_Click(object sender, EventArgs e)
        {
            SelectChannels selectChannels = new SelectChannels();
            if(selectChannels.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            for (int channelIdx = 0; channelIdx < selectChannels.selectedChannelNames.Count; channelIdx++)
            {
                channels.Add(new ChannelDisplayInfo(selectChannels.selectedChannelSessions[channelIdx],
                                                    YamuraViewMain.dataLogger.sessionData[selectChannels.selectedChannelSessions[channelIdx]].channels[selectChannels.selectedChannelNames[channelIdx]],
                                                    YamuraViewMain.dataLogger.sessionData[selectChannels.selectedChannelSessions[channelIdx]].channels["Time"]));
                channels[channels.Count - 1].ChannelColor = colors[(channels.Count - 1) % colors.Count];
                channels[channels.Count - 1].ShowChannel = true;
                if (channels[channels.Count - 1].SessionIdx == 0)
                {
                    channels[channels.Count - 1].AxisOffsetX[0] = 50.0F;
                }
                channelListView.Rows.Add();
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[0].Value = channels[channels.Count - 1].ShowChannel;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[1].Style.BackColor = channels[channels.Count - 1].ChannelColor;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[2].Value = channels[channels.Count - 1].dataChannel.ChannelName;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[3].Value = channels[channels.Count - 1].SessionIdx.ToString();
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[4].Value = (channels[channels.Count - 1].dataChannel.dataPoints.ElementAt(0).Value as DataPoint).PointValue.ToString();

            }
            chartPanel.Invalidate();
        }

        private void removeChannelsMenuItem_Click(object sender, EventArgs e)
        {
            chartPanel.Invalidate();
        }

        private void xAxisMenuItem_Click(object sender, EventArgs e)
        {
            SelectChannels selectChannels = new SelectChannels();
            if (selectChannels.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            string xAxisName = selectChannels.selectedChannelNames[0];
            int sessionIdx = 0;
            for (int channelIdx = 0; channelIdx < channels.Count(); channelIdx++)
            {
                sessionIdx = channels[channelIdx].SessionIdx;
                channels[channelIdx].axisChannel = YamuraViewMain.dataLogger.sessionData[sessionIdx].channels[xAxisName];
                channels[channelIdx].ChannelPath = new GraphicsPath();
            }
            chartPanel.Invalidate();
        }

        private void channelListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // show/hide
            if (e.ColumnIndex == 0)
            {
                channelListView.CommitEdit(DataGridViewDataErrorContexts.Commit);

                int sessionIdx = Convert.ToInt32(channelListView.Rows[e.RowIndex].Cells[3].Value);
                string channelName = Convert.ToString(channelListView.Rows[e.RowIndex].Cells[2].Value);

                System.Diagnostics.Debug.Write("CellContentClick row " + e.RowIndex + " " + e.ColumnIndex);
                System.Diagnostics.Debug.Write(" Cell value " + channelListView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                System.Diagnostics.Debug.WriteLine(" channel " + channelName);
                System.Diagnostics.Debug.WriteLine(" session " + sessionIdx);
                foreach(ChannelDisplayInfo channel in channels)
                {
                    if((channel.SessionIdx == sessionIdx) && (channel.dataChannel.ChannelName == channelName))
                    {
                        channel.ShowChannel = (bool)channelListView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    }
                }
                chartPanel.Invalidate();
            }
        }

        private void channelListView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // set color
            if (e.ColumnIndex == 1)
            {
                if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                {
                    return;
                }
                int sessionIdx = Convert.ToInt32(channelListView.Rows[e.RowIndex].Cells[3].Value);
                string channelName = Convert.ToString(channelListView.Rows[e.RowIndex].Cells[2].Value);
                foreach (ChannelDisplayInfo channel in channels)
                {
                    if ((channel.SessionIdx == sessionIdx) && (channel.dataChannel.ChannelName == channelName))
                    {
                        channel.ChannelColor = colorDialog1.Color;
                        break;
                    }
                }
                channelListView.Rows[e.RowIndex].Cells[1].Style.BackColor = colorDialog1.Color;
                chartPanel.Invalidate();

            }
        }

        private void splitContainer1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == '+')
            //{
            //    foreach (ChannelDisplayInfo channel in channels)
            //    {
            //        if (channel.SessionIdx == 0)
            //        {
            //            channel.AxisOffsetX[0] += 50.0F;
            //        }
            //    }
            //    chartPanel.Invalidate();
            //}
        }

        private void splitContainer1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {
                foreach (ChannelDisplayInfo channel in channels)
                {
                    if (channel.SessionIdx == 0)
                    {
                        channel.AxisOffsetX[0] += 1.0F;
                    }
                }
                chartPanel.Invalidate();
            }
            else if (e.KeyCode == Keys.Left)
            {
                foreach (ChannelDisplayInfo channel in channels)
                {
                    if (channel.SessionIdx == 0)
                    {
                        channel.AxisOffsetX[0] -= 1.0F;
                    }
                }
                chartPanel.Invalidate();

            }
        }

        private void chartPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                if (dragStart == true)
                {
                    foreach (ChannelDisplayInfo channel in channels)
                    {
                        if (channel.SessionIdx == dragSession)
                        {
                            channel.AxisOffsetX[0] += mouseLast > e.X ? -1 : 1;
                        }
                    }
                    chartPanel.Invalidate();
                }
                else
                {
                    dragStart = true;
                    int curRow = channelListView.SelectedRows[0].Index;
                    dragSession = Convert.ToInt32( channelListView.Rows[curRow].Cells[3].Value);

                }
                mouseLast = e.Location.X;

            }
        }

        private void chartPanel_MouseUp(object sender, MouseEventArgs e)
        {
            dragStart = false;
        }
    }
}
