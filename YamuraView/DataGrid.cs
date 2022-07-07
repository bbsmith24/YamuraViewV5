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
        int curSessionIdx = 0;
        public DataGrid()
        {
            InitializeComponent();
        }

        private void DataGrid_Activated(object sender, EventArgs e)
        {
            foreach(SessionData curSession in YamuraViewMain.dataLogger.sessionData)
            {
                if(!toolStripSessionList.Items.Contains(curSession.fileName))
                {
                    toolStripSessionList.Items.Add(curSession.fileName);
                }
            }
        }

        private void toolStripSessionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            curSessionIdx = toolStripSessionList.SelectedIndex;
            for(int colIdx = dataGridValues.ColumnCount - 1; colIdx > 0; colIdx--)
            {
                dataGridValues.Columns.RemoveAt(colIdx);
            }
            Text = "Data Grid: " + YamuraViewMain.dataLogger.sessionData[curSessionIdx].fileName;
            channelListView.Rows.Clear();
            int rowIdx = 0;
            foreach (KeyValuePair<string, DataChannel> channel in YamuraViewMain.dataLogger.sessionData[curSessionIdx].channels)
            {
                channelListView.Rows.Add();
                channelListView.Rows[rowIdx].Cells[0].Value = false;// channels[channels.Count - 1].ShowChannel;
                channelListView.Rows[rowIdx].Cells[1].Value = channel.Key.ToString();// channels[channels.Count - 1].dataChannel.ChannelName + " (" + channels[channels.Count - 1].SessionIdx.ToString() + ")";
                rowIdx++;
            }


            dataGridValues.Rows.Clear();
            rowIdx = 0;
            DataChannel timeChannel = YamuraViewMain.dataLogger.sessionData[curSessionIdx].channels["Time"];
            foreach (KeyValuePair<float, DataPoint> dataPoint in timeChannel.dataPoints)
            {
                dataGridValues.Rows.Add();
                dataGridValues.Rows[rowIdx].Cells[0].Value = dataPoint.Key.ToString("0.000");
                rowIdx++;
            }
        }
    }
}
