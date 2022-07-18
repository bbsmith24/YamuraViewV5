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
        List<ChannelDisplayInfo> channels = new List<ChannelDisplayInfo>();

        public TrackMap()
        {
            InitializeComponent();
            this.Text = "Track Map";
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
                if ((channel.ChannelPath == null) ||
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
            float[] yAxisRange = new float[2] { float.MaxValue, float.MinValue };
            Dictionary<string, float[]> yAxesRanges = new Dictionary<string, float[]>();
            //float[] yAxisRange = new float[2] { float.MaxValue, float.MinValue };
            int sessionIdx = 0;
            foreach (ChannelDisplayInfo channel in channels)
            {
                #region skip if channel is not displayed
                //if (!channel.ShowChannel)
                if((bool)dataGridView1.Rows[sessionIdx].Cells["colShowSession"].Value == false)
                {
                    sessionIdx++;
                    continue;
                }
                sessionIdx++;
                #endregion
                System.Diagnostics.Debug.WriteLine(channel.dataChannel.ChannelName + " (" + channel.SessionIdx + ")");
                System.Diagnostics.Debug.WriteLine("X axis " + channel.axisChannel.DataRange[0] + " to " + channel.axisChannel.DataRange[1]);
                System.Diagnostics.Debug.WriteLine("Y axis " + channel.dataChannel.DataRange[0] + " to " + channel.dataChannel.DataRange[1]);

                xAxisRange[0] = xAxisRange[0] < channel.axisChannel.DataRange[0] + channel.AxisOffsetX[0] ? xAxisRange[0] : channel.axisChannel.DataRange[0] + channel.AxisOffsetX[0];
                xAxisRange[1] = xAxisRange[1] > channel.axisChannel.DataRange[1] + channel.AxisOffsetX[0] ? xAxisRange[1] : channel.axisChannel.DataRange[1] + channel.AxisOffsetX[0];

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

            //xAxisRange[0] = 0.0F;
            System.Diagnostics.Debug.WriteLine("Overall");
            System.Diagnostics.Debug.WriteLine("X axis " + xAxisRange[0] + " to " + xAxisRange[1]);
            foreach (KeyValuePair<string, float[]> yAxis in yAxesRanges)
            {
                yAxisRange[0] = yAxisRange[0] < yAxis.Value[0] ? yAxisRange[0] : yAxis.Value[0];
                yAxisRange[1] = yAxisRange[1] > yAxis.Value[1] ? yAxisRange[1] : yAxis.Value[1];
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
            sessionIdx = 0;
            foreach (ChannelDisplayInfo channel in channels)
            {
                #region skip if channel is not displayed
                if ((bool)dataGridView1.Rows[sessionIdx].Cells["colShowSession"].Value == false)
                {
                    sessionIdx++;
                    continue;
                }
                #endregion
                #region draw to transformed graphic context
                pathPen = new Pen(YamuraViewMain.dataLogger.sessionData[sessionIdx].sessionColor == null ? Color.Red : YamuraViewMain.dataLogger.sessionData[sessionIdx].sessionColor);
                using (Graphics chartGraphics = chartPanel.CreateGraphics())
                {
                    Rectangle clipRect = chartPanel.Bounds;
                    clipRect.Inflate(-5, -5);
                    chartGraphics.SetClip(clipRect);

                    //displayScale[0] = (float)clipRect.Width / (channel.axisChannel.DataRange[1] - channel.axisChannel.DataRange[0]);
                    //displayScale[1] = (float)clipRect.Height / (channel.dataChannel.DataRange[1] - channel.dataChannel.DataRange[0]);
                    displayScale[0] = (float)clipRect.Width / (xAxisRange[1] - xAxisRange[0]);
                    float yRange = yAxisRange[1] - yAxisRange[0];//(yAxesRanges[channel.dataChannel.ChannelName][1] - yAxesRanges[channel.dataChannel.ChannelName][0]);
                    yRange *= 1.01F;
                    displayScale[1] = (float)clipRect.Height / yRange;
                    if(displayScale[1] > displayScale[0])
                    {
                        displayScale[1] = displayScale[0];
                    }
                    else
                    {
                        displayScale[0] = displayScale[1];
                    }

                    displayScale[1] *= -1.0F;
                    // translate to lower left corner of display area
                    //chartGraphics.TranslateTransform(chartBorder, (float)clipRect.Height/* + chartBorder*/);
                    // scale to display range in X and Y
                    chartGraphics.ScaleTransform(displayScale[0], displayScale[1]);
                    // this works, inverted in Y and no traslation
                    //chartGraphics.TranslateTransform(-1.0F * xAxisRange[0],
                    //                                 -1.0F * yAxesRanges[channel.dataChannel.ChannelName][0]);

                    // from prior version
                    //chartGraphics.TranslateTransform(-1 * ChartOwner.ChartAxes[xChannelName].AxisDisplayRange[0] +
                    //                                 ChartOwner.ChartAxes[xChannelName].AssociatedChannels[curChanInfo.Value.RunIndex.ToString() + "-" + xChannelName].AxisOffset[0],  // offset X
                    //                                 -1 * yAxis.Value.AxisDisplayRange[0] +
                    //                                 ChartOwner.ChartAxes[xChannelName].AssociatedChannels[curChanInfo.Value.RunIndex.ToString() + "-" + xChannelName].AxisOffset[1]);  // offset Y
                    chartGraphics.TranslateTransform(-1 * xAxisRange[0] + channel.AxisOffsetX[0],
                                                     -1 * (yAxesRanges[channel.dataChannel.ChannelName][1] + (yRange * 0.01F)));// channel.AxisOffsetY[0]);



                    pathPen.Width = 0;
                    // draw the path
                    chartGraphics.DrawPath(pathPen, channel.ChannelPath);
                    // reset to original orientation
                    chartGraphics.ResetTransform();
                }
                sessionIdx++;
                #endregion
            }
            #endregion
        }
        private void chartPanel_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void TrackMap_Activated(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            channels.Clear();
            for(int idx = 0; idx < YamuraViewMain.dataLogger.sessionData.Count; idx++)
            {
                if ((YamuraViewMain.dataLogger.sessionData[idx].channels.ContainsKey("Latitude")) &&
                    (YamuraViewMain.dataLogger.sessionData[idx].channels.ContainsKey("Longitude")))
                {
                    dataGridView1.Rows.Add();
                    dataGridView1.Rows[idx].Cells["colColor"].Style.BackColor = YamuraViewMain.dataLogger.sessionData[idx].sessionColor;
                    dataGridView1.Rows[idx].Cells["colShowSession"].Value = true;
                    dataGridView1.Rows[idx].Cells["colFile"].Value = YamuraViewMain.dataLogger.sessionData[idx].fileName;
                    dataGridView1.Rows[idx].Cells["colSession"].Value = idx;
                    channels.Add(new ChannelDisplayInfo(idx,
                                                        YamuraViewMain.dataLogger.sessionData[idx].channels["Latitude"],
                                                        YamuraViewMain.dataLogger.sessionData[idx].channels["Longitude"]));
                }
            }
        }
    }
}
