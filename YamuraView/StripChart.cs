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
    public partial class StripChart : Form
    {
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

                    displayScale[0] = (float)clipRect.Width / (channel.axisChannel.DataRange[1] - channel.axisChannel.DataRange[0]);
                    displayScale[1] = (float)clipRect.Height / (channel.dataChannel.DataRange[1] - channel.dataChannel.DataRange[0]);
                    displayScale[1] *= -1.0F;
                    // translate to lower left corner of display area
                    //chartGraphics.TranslateTransform(chartBorder, (float)clipRect.Height/* + chartBorder*/);
                    // scale to display range in X and Y
                    chartGraphics.ScaleTransform(displayScale[0], displayScale[1]);
                    // translate by -1 * minimum display range + axis offset (scrolling)
                    chartGraphics.TranslateTransform(0.0F, //-1 * axisChannel.dataChannel.DataRange[0],
                    //                                    //+ ChartOwner.ChartAxes[xChannelName].AssociatedChannels[curChanInfo.SessionIndex.ToString() + "-" + xChannelName].AxisOffset[0],  // offset X
                                                        -1 * channel.dataChannel.DataRange[1]);// yAxis.Value.AxisDisplayRange[0]);
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
                                                    YamuraViewMain.dataLogger.sessionData[selectChannels.selectedChannelSessions[0]].channels["Time"]));
                channels[channels.Count - 1].ChannelColor = colors[(channels.Count - 1) % colors.Count];
                channels[channels.Count - 1].ShowChannel = true;
                channelListView.Rows.Add();
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[0].Value = channels[channels.Count - 1].ShowChannel;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[1].Style.BackColor = channels[channels.Count - 1].ChannelColor;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[2].Value = channels[channels.Count - 1].dataChannel.ChannelName + " (" + channels[channels.Count - 1].SessionIdx.ToString() + ")";
                channelListView.Rows[channelListView.Rows.Count - 1].Cells[3].Value = (channels[channels.Count - 1].dataChannel.dataPoints.ElementAt(0).Value as DataPoint).PointValue.ToString();

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
            for (int channelIdx = 0; channelIdx < channels.Count(); channelIdx++)
            {
                channels[channelIdx].axisChannel = YamuraViewMain.dataLogger.sessionData[selectChannels.selectedChannelSessions[0]].channels[selectChannels.selectedChannelNames[0]];
            }
            chartPanel.Invalidate();
        }
    }
}
