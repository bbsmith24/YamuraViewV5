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
    public partial class SelectChannels : Form
    {
        public List<string> selectedChannelNames = new List<string>();
        public List<int> selectedChannelSessions = new List<int>();
        public SelectChannels()
        {
            InitializeComponent();
            int sessionCount = 0;
            int channelCount = 0;
            string[] sessionNameParts;
            foreach(SessionData session in YamuraViewMain.dataLogger.sessionData)
            {
                sessionNameParts = session.fileName.Split(new char[] { '\\', '.' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (KeyValuePair<string, DataChannel> channel in session.channels)
                {
                    dataGridChannelSelect.Rows.Add();
                    //dataGridChannelSelect.Rows[channelCount].Cells[0] = channel.Key;
                    dataGridChannelSelect.Rows[channelCount].Cells[0].Value = channel.Key;
                    dataGridChannelSelect.Rows[channelCount].Cells[1].Value = sessionNameParts[sessionNameParts.Count() - 2];
                    dataGridChannelSelect.Rows[channelCount].Cells[2].Value = sessionCount + 1;
                    dataGridChannelSelect.Rows[channelCount].Cells[3].Value = channel.Value.dataPoints.Count;
                    dataGridChannelSelect.Rows[channelCount].Cells[4].Value = channel.Value.TimeRange[0];
                    dataGridChannelSelect.Rows[channelCount].Cells[5].Value = channel.Value.TimeRange[1];
                    channelCount++;
                }
                sessionCount++;
            }
            dataGridChannelSelect.Sort(dataGridChannelSelect.Columns[0], ListSortDirection.Ascending);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            selectedChannelNames.Clear();
            selectedChannelSessions.Clear();
            foreach (DataGridViewRow dataRow in  dataGridChannelSelect.SelectedRows)
            {
                selectedChannelNames.Add((string)dataRow.Cells[0].Value);
                selectedChannelSessions.Add((int)dataRow.Cells[2].Value - 1);
            }
        }
    }
}
