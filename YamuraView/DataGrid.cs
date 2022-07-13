using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YamuraView
{
    public partial class DataGrid : Form
    {
        List<ChannelDisplayInfo> channels = new List<ChannelDisplayInfo>();
        int curSessionIdx = -1;
        public DataGrid()
        {
            InitializeComponent();
        }

        private void DataGrid_Activated(object sender, EventArgs e)
        {
            toolStripSessionList.Items.Clear();
            foreach (SessionData curSession in YamuraViewMain.dataLogger.sessionData)
            {
                if (!toolStripSessionList.Items.Contains(curSession.fileName))
                {
                    toolStripSessionList.Items.Add(curSession.fileName);
                }
            }
            if((toolStripSessionList.Items.Count > 0) && (curSessionIdx >= 0))
            {
                toolStripSessionList.SelectedIndex = curSessionIdx;
            }
        }

        private void toolStripSessionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            curSessionIdx = toolStripSessionList.SelectedIndex;
            for (int colIdx = dataGridValues.ColumnCount - 1; colIdx > 0; colIdx--)
            {
                dataGridValues.Columns.RemoveAt(colIdx);
            }
            Text = "Data Grid: " + YamuraViewMain.dataLogger.sessionData[curSessionIdx].fileName;
            channelListView.Rows.Clear();
            channels.Clear();
            int rowIdx = 0;
            foreach (KeyValuePair<string, DataChannel> channel in YamuraViewMain.dataLogger.sessionData[curSessionIdx].channels)
            {
                channelListView.Rows.Add();
                channelListView.Rows[rowIdx].Cells[0].Value = false;
                channelListView.Rows[rowIdx].Cells[1].Value = channel.Key.ToString();
                channels.Add(new ChannelDisplayInfo(curSessionIdx,
                                                    channel.Value,
                                                    null));
                if(channel.Key == "Time")
                {
                    channels[rowIdx].ShowChannel = true;
                    channelListView.Rows[rowIdx].Cells[0].Value = true;
                }
                rowIdx++;
            }

            dataGridValues.Rows.Clear();
            rowIdx = 0;
            DataChannel timeChannel = YamuraViewMain.dataLogger.sessionData[curSessionIdx].channels["Time"];
            Cursor = Cursors.WaitCursor;
            foreach (KeyValuePair<float, DataPoint> dataPoint in timeChannel.dataPoints)
            {
                dataGridValues.Rows.Add();
                dataGridValues.Rows[rowIdx].Cells[0].Value = dataPoint.Value.PointValue;//.ToString("0.000");
                rowIdx++;
            }
            Cursor = Cursors.Default;
        }

        private void channelListView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // show/hide
            if (e.ColumnIndex == 0)
            {
                channelListView.CommitEdit(DataGridViewDataErrorContexts.Commit);

                //int sessionIdx =  Convert.ToInt32(channelListView.Rows[e.RowIndex].Cells[0].Value);
                string channelName = Convert.ToString(channelListView.Rows[e.RowIndex].Cells[1].Value);

                foreach (ChannelDisplayInfo channel in channels)
                {
                    if (/*(channel.SessionIdx == sessionIdx) && */(channel.dataChannel.ChannelName == channelName))
                    {
                        channel.ShowChannel = (bool)channelListView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                    }
                }
                dataGridValues.Rows.Clear();
                dataGridValues.Columns.Clear();
                Cursor = Cursors.WaitCursor;
                int rowIdx = 0;
                int colIdx = 0;
                foreach (ChannelDisplayInfo channel in channels)
                {
                    if (channel.ShowChannel == false)
                    {
                        continue;
                    }
                    dataGridValues.Columns.Add(channel.dataChannel.ChannelName, channel.dataChannel.ChannelName);
                    colIdx = dataGridValues.Columns.Count - 1;
                    rowIdx = 0;
                    foreach (KeyValuePair<float, DataPoint> dataPoint in channel.dataChannel.dataPoints)
                    {
                        if (rowIdx <= dataGridValues.Rows.Count)
                        {
                            dataGridValues.Rows.Add();
                        }
                        if (channel.dataChannel.ChannelName == "Time")
                        {
                            dataGridValues.Rows[rowIdx].Cells[colIdx].Value = dataPoint.Value.PointValue;//.ToString("0.000");
                        }
                        else
                        {
                            while((float)dataGridValues.Rows[rowIdx].Cells[0].Value != dataPoint.Key)
                            {
                                rowIdx++;
                            }
                            dataGridValues.Rows[rowIdx].Cells[colIdx].Value = dataPoint.Value.PointValue;//.ToString("0.000");
                        }
                        rowIdx++;
                    }
                }
                Cursor = Cursors.Default;
            }
        }
    }
}
