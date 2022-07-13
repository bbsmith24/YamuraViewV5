namespace YamuraView
{
    partial class DataGrid
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridValues = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.channelListView = new System.Windows.Forms.DataGridView();
            this.colShow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colChannel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripSelectSession = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSessionList = new System.Windows.Forms.ToolStripComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.channelListView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridValues);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.channelListView);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 266;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridValues
            // 
            this.dataGridValues.AllowUserToAddRows = false;
            this.dataGridValues.AllowUserToDeleteRows = false;
            this.dataGridValues.AllowUserToResizeRows = false;
            this.dataGridValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime});
            this.dataGridValues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridValues.Location = new System.Drawing.Point(0, 0);
            this.dataGridValues.Name = "dataGridValues";
            this.dataGridValues.RowHeadersVisible = false;
            this.dataGridValues.Size = new System.Drawing.Size(266, 450);
            this.dataGridValues.TabIndex = 0;
            // 
            // colTime
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colTime.DefaultCellStyle = dataGridViewCellStyle4;
            this.colTime.HeaderText = "Time";
            this.colTime.Name = "colTime";
            // 
            // channelListView
            // 
            this.channelListView.AllowUserToAddRows = false;
            this.channelListView.AllowUserToDeleteRows = false;
            this.channelListView.AllowUserToResizeRows = false;
            this.channelListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.channelListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShow,
            this.colChannel});
            this.channelListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.channelListView.Location = new System.Drawing.Point(0, 0);
            this.channelListView.Name = "channelListView";
            this.channelListView.RowHeadersWidth = 10;
            this.channelListView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.channelListView.Size = new System.Drawing.Size(530, 450);
            this.channelListView.TabIndex = 1;
            this.channelListView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.channelListView_CellContentClick);
            // 
            // colShow
            // 
            this.colShow.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.colShow.HeaderText = "";
            this.colShow.Name = "colShow";
            this.colShow.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colShow.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colShow.Width = 5;
            // 
            // colChannel
            // 
            this.colChannel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colChannel.HeaderText = "Channel";
            this.colChannel.Name = "colChannel";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSelectSession});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // toolStripSelectSession
            // 
            this.toolStripSelectSession.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSessionList});
            this.toolStripSelectSession.Name = "toolStripSelectSession";
            this.toolStripSelectSession.Size = new System.Drawing.Size(92, 20);
            this.toolStripSelectSession.Text = "Select Session";
            // 
            // toolStripSessionList
            // 
            this.toolStripSessionList.Items.AddRange(new object[] {
            "test1",
            "test2",
            "test3"});
            this.toolStripSessionList.Name = "toolStripSessionList";
            this.toolStripSessionList.Size = new System.Drawing.Size(121, 23);
            this.toolStripSessionList.SelectedIndexChanged += new System.EventHandler(this.toolStripSessionList_SelectedIndexChanged);
            // 
            // DataGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DataGrid";
            this.Text = "Data Grid";
            this.Activated += new System.EventHandler(this.DataGrid_Activated);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.channelListView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridValues;
        private System.Windows.Forms.DataGridView channelListView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChannel;
        private System.Windows.Forms.ToolStripMenuItem toolStripSelectSession;
        private System.Windows.Forms.ToolStripComboBox toolStripSessionList;
    }
}