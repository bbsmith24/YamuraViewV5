namespace YamuraView
{
    partial class StripChart
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
            this.chartPanel = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.channelsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChannelsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeChannelsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xAxisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.channelListView = new System.Windows.Forms.DataGridView();
            this.colShow = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colColor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChannel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.channelListView)).BeginInit();
            this.SuspendLayout();
            // 
            // chartPanel
            // 
            this.chartPanel.BackColor = System.Drawing.Color.Black;
            this.chartPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartPanel.Location = new System.Drawing.Point(0, 0);
            this.chartPanel.Name = "chartPanel";
            this.chartPanel.Size = new System.Drawing.Size(556, 450);
            this.chartPanel.TabIndex = 0;
            this.chartPanel.SizeChanged += new System.EventHandler(this.chartPanel_SizeChanged);
            this.chartPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.chartPanel_Paint);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.channelsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // channelsToolStripMenuItem
            // 
            this.channelsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addChannelsMenuItem,
            this.removeChannelsMenuItem,
            this.xAxisToolStripMenuItem});
            this.channelsToolStripMenuItem.Name = "channelsToolStripMenuItem";
            this.channelsToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.channelsToolStripMenuItem.Text = "Channels";
            // 
            // addChannelsMenuItem
            // 
            this.addChannelsMenuItem.Name = "addChannelsMenuItem";
            this.addChannelsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addChannelsMenuItem.Text = "Add Channel(s)";
            this.addChannelsMenuItem.Click += new System.EventHandler(this.addChannelsMenuItem_Click);
            // 
            // removeChannelsMenuItem
            // 
            this.removeChannelsMenuItem.Name = "removeChannelsMenuItem";
            this.removeChannelsMenuItem.Size = new System.Drawing.Size(180, 22);
            this.removeChannelsMenuItem.Text = "Remove Channel(s)";
            this.removeChannelsMenuItem.Click += new System.EventHandler(this.removeChannelsMenuItem_Click);
            // 
            // xAxisToolStripMenuItem
            // 
            this.xAxisToolStripMenuItem.Name = "xAxisToolStripMenuItem";
            this.xAxisToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.xAxisToolStripMenuItem.Text = "X Axis";
            this.xAxisToolStripMenuItem.Click += new System.EventHandler(this.xAxisMenuItem_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.chartPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.channelListView);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 556;
            this.splitContainer1.TabIndex = 3;
            // 
            // channelListView
            // 
            this.channelListView.AllowUserToAddRows = false;
            this.channelListView.AllowUserToDeleteRows = false;
            this.channelListView.AllowUserToResizeRows = false;
            this.channelListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.channelListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colShow,
            this.colColor,
            this.colChannel,
            this.colValue});
            this.channelListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.channelListView.Location = new System.Drawing.Point(0, 0);
            this.channelListView.Name = "channelListView";
            this.channelListView.RowHeadersVisible = false;
            this.channelListView.Size = new System.Drawing.Size(240, 450);
            this.channelListView.TabIndex = 0;
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
            // colColor
            // 
            this.colColor.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colColor.HeaderText = "";
            this.colColor.Name = "colColor";
            this.colColor.Width = 20;
            // 
            // colChannel
            // 
            this.colChannel.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colChannel.HeaderText = "Channel";
            this.colChannel.Name = "colChannel";
            this.colChannel.Width = 71;
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            // 
            // StripChart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "StripChart";
            this.Text = "Strip Chart";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.channelListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel chartPanel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem channelsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addChannelsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeChannelsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xAxisToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView channelListView;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colShow;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChannel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
    }
}