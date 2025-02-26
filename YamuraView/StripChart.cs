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
        bool dragStart = false;
        int dragSession = 0;
        int mouseLast = 0;
        float[] displayScale = new float[2] { 0.0F, 0.0F };
        float[,] axisRange = new float[2,2] { { 0.0F, 0.0F }, { 0.0F, 0.0F } };
        float[] axisOffset = new float[2] { 0.0F, 0.0F };
        List<ChannelDisplayInfo> channels = new List<ChannelDisplayInfo>();
        public StripChart()
        {
            InitializeComponent();
            this.Text = "Strip Chart";
            chartPanel.BringToFront();
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
            axisRange = new float[2, 2] {{ float.MaxValue, float.MinValue },{ float.MaxValue, float.MinValue } };
            Dictionary<string, float[]> yAxesRanges = new Dictionary<string, float[]>();
            foreach (ChannelDisplayInfo channel in channels)
            {
                #region skip if channel is not displayed
                if (!channel.ShowChannel)
                {
                    continue;
                }
                #endregion

                axisRange[0, 0] = axisRange[0, 0] < channel.axisChannel.DataRange[0] + channel.AxisOffsetX[0] ? axisRange[0, 0] : channel.axisChannel.DataRange[0] + channel.AxisOffsetX[0];
                axisRange[0, 1] = axisRange[0, 1] > channel.axisChannel.DataRange[1] + channel.AxisOffsetX[0] ? axisRange[0, 1] : channel.axisChannel.DataRange[1] + channel.AxisOffsetX[0];
                axisRange[1, 0] = axisRange[1, 0] < channel.dataChannel.DataRange[0] ? axisRange[1, 0] : channel.dataChannel.DataRange[0];
                axisRange[1, 1] = axisRange[1, 1] > channel.axisChannel.DataRange[1] ? axisRange[1, 1] : channel.dataChannel.DataRange[1];

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

            foreach (KeyValuePair<string, float[]> yAxis in yAxesRanges)
            {
                axisRange[1, 0] = axisRange[1, 0] < yAxis.Value[0] ? axisRange[1, 0] : yAxis.Value[0];
                axisRange[1, 1] = axisRange[1, 1] > yAxis.Value[1] ? axisRange[1, 1] : yAxis.Value[1];
            }
            #endregion
            #region draw path to display
            Pen pathPen = new Pen(Color.Red);
            int width = chartPanel.Width;
            int height = chartPanel.Height;
            // x and y scale
            //displayScale = new float[] { 1.0F, 1.0F };
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
                    displayScale[0] = (float)clipRect.Width / (axisRange[0, 1] - axisRange[0, 0]);
                    float yRange = axisRange[1, 1] - axisRange[1, 0];//(yAxesRanges[channel.dataChannel.ChannelName][1] - yAxesRanges[channel.dataChannel.ChannelName][0]);
                    yRange *= 1.01F;
                    displayScale[1] = (float)clipRect.Height / yRange;
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
                    chartGraphics.TranslateTransform(-1 * axisRange[0, 0] + channel.AxisOffsetX[0],
                                                     -1 * (yAxesRanges[channel.dataChannel.ChannelName][1] + (yRange * 0.01F)));// channel.AxisOffsetY[0]);
                    


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
                channels[channels.Count - 1].ChannelColor = YamuraViewMain.colors[(channels.Count - 1) % YamuraViewMain.colors.Count];
                channels[channels.Count - 1].ShowChannel = true;
                channelListView.Rows.Add();
                
                String shortName = YamuraViewMain.dataLogger.sessionData[channels[channels.Count - 1].SessionIdx].fileName;
                int lastSlash = shortName.LastIndexOf('\\');
                if (lastSlash >= 0)
                {
                    shortName = shortName.Substring(lastSlash + 1);
                }

                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colShow"].Value = channels[channels.Count - 1].ShowChannel;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colColor"].Style.BackColor = channels[channels.Count - 1].ChannelColor;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colCHannel"].Value = channels[channels.Count - 1].dataChannel.ChannelName;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colSession"].Value = channels[channels.Count - 1].SessionIdx.ToString();
                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colFileName"].Value = shortName;
                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colValue"].Value = (channels[channels.Count - 1].dataChannel.dataPoints.ElementAt(0).Value as DataPoint).PointValue.ToString();
                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colMinVal"].Value = (channels[channels.Count - 1].dataChannel.DataRange[0].ToString());
                channelListView.Rows[channelListView.Rows.Count - 1].Cells["colMaxVal"].Value = (channels[channels.Count - 1].dataChannel.DataRange[1].ToString());

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
            #region drag to align sessions
            if (e.Button == MouseButtons.Left)
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
                    dragSession = Convert.ToInt32(channelListView.Rows[curRow].Cells[3].Value);

                }
                mouseLast = e.Location.X;

            }
            #endregion
            #region mouse move to update values at position and cursor in other views
            else if ((e.Button == MouseButtons.None) && 
                     (channels.Count > 0))
            {
                DataChannel xAxisChannel = channels[0].axisChannel;// YamuraViewMain.dataLogger.sessionData[0].channels["Time"];
                float[] cursorPos = new float[2]  { (float)e.Location.X, (float)e.Location.Y };
                float[] cursorOffset = new float[2] { -1 * axisRange[0, 0] + 0.0F, 0.0F };
                cursorPos[0] = (cursorPos[0] / displayScale[0]) + cursorOffset[0];
                DataPoint channelValue;
                float findLowestAbove = 0.0F;
                float findHighestBelow = 0.0F;
                float findAxisTime = 0.0F;
System.Diagnostics.Debug.Write("Cursor at " + cursorPos[0].ToString());
                for (int channelIdx = 0; channelIdx < channels.Count; channelIdx++)
                {
                    System.Diagnostics.Debug.Write(" " + channels[channelIdx].dataChannel.ChannelName + " ");
                    if(xAxisChannel.ChannelName == "Time")
                    {
                        findAxisTime = cursorPos[0];
System.Diagnostics.Debug.Write("(time)\t");
                    }
                    else
                    {
System.Diagnostics.Debug.Write("(cursor dist)\t");
                        findLowestAbove = YamuraViewMain.dataLogger.sessionData[channels[channelIdx].SessionIdx].channels["Distance-Time"].DataPoints.Keys.Where(x => x > cursorPos[0]).FirstOrDefault();
                        findHighestBelow = YamuraViewMain.dataLogger.sessionData[channels[channelIdx].SessionIdx].channels["Distance-Time"].DataPoints.Keys.OrderByDescending(x => x).Where(x => x > cursorPos[0]).FirstOrDefault();
System.Diagnostics.Debug.Write(findLowestAbove.ToString() + " - " + findHighestBelow .ToString() + " (found dist)\t");
                        findAxisTime = YamuraViewMain.dataLogger.sessionData[channels[channelIdx].SessionIdx].channels["Distance-Time"].DataPoints[findLowestAbove].PointValue;
                        cursorPos[0] = channels[channelIdx].dataChannel.DataPoints[findAxisTime].PointValue;
//System.Diagnostics.Debug.Write("Cursor at " + xAxisChannel.ChannelName + " " + findAxisTime + "ms\t"+ cursorPos[0].ToString() + "s\t");

                    }
                    findLowestAbove = channels[channelIdx].dataChannel.DataPoints.Keys.Where(x => x > findAxisTime).FirstOrDefault();
                    findHighestBelow = channels[channelIdx].dataChannel.DataPoints.Keys.OrderByDescending(x => x).Where(x => x < findAxisTime).FirstOrDefault();

System.Diagnostics.Debug.Write(" found time " + findHighestBelow.ToString() + " channel value ");

                    if (channels[channelIdx].dataChannel.DataPoints.TryGetValue(findHighestBelow, out channelValue))
                    {
                        System.Diagnostics.Debug.Write(channelValue.PointValue.ToString() + " ");
                    }
                    else
                    {
                        System.Diagnostics.Debug.Write("--- ");
                    }
System.Diagnostics.Debug.WriteLine(" ");
                }
            };
            #endregion
        }

        private void chartPanel_MouseUp(object sender, MouseEventArgs e)
        {
            dragStart = false;
        }
    }
}
