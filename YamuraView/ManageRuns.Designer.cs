namespace YamuraView
{
    partial class ManageSessions
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridSessions = new System.Windows.Forms.DataGridView();
            this.colSession = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStart = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colChannels = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.openLogFile = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.sessionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSessions = new System.Windows.Forms.ToolStripMenuItem();
            this.removeSessions = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSessions)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridSessions
            // 
            this.dataGridSessions.AllowUserToAddRows = false;
            this.dataGridSessions.AllowUserToDeleteRows = false;
            this.dataGridSessions.AllowUserToResizeRows = false;
            this.dataGridSessions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSessions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSession,
            this.colDateTime,
            this.colStart,
            this.colEnd,
            this.colChannels,
            this.colSource});
            this.dataGridSessions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridSessions.Location = new System.Drawing.Point(0, 24);
            this.dataGridSessions.Name = "dataGridSessions";
            this.dataGridSessions.Size = new System.Drawing.Size(800, 426);
            this.dataGridSessions.TabIndex = 2;
            // 
            // colSession
            // 
            this.colSession.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSession.DefaultCellStyle = dataGridViewCellStyle1;
            this.colSession.HeaderText = "Session";
            this.colSession.Name = "colSession";
            this.colSession.Width = 69;
            // 
            // colDateTime
            // 
            this.colDateTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colDateTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.colDateTime.HeaderText = "Date/Time";
            this.colDateTime.Name = "colDateTime";
            this.colDateTime.Width = 83;
            // 
            // colStart
            // 
            this.colStart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colStart.DefaultCellStyle = dataGridViewCellStyle3;
            this.colStart.HeaderText = "Start";
            this.colStart.Name = "colStart";
            this.colStart.Width = 54;
            // 
            // colEnd
            // 
            this.colEnd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colEnd.DefaultCellStyle = dataGridViewCellStyle4;
            this.colEnd.HeaderText = "End";
            this.colEnd.Name = "colEnd";
            this.colEnd.Width = 51;
            // 
            // colChannels
            // 
            this.colChannels.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colChannels.DefaultCellStyle = dataGridViewCellStyle5;
            this.colChannels.HeaderText = "Channels";
            this.colChannels.Name = "colChannels";
            this.colChannels.Width = 76;
            // 
            // colSource
            // 
            this.colSource.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.colSource.DefaultCellStyle = dataGridViewCellStyle6;
            this.colSource.HeaderText = "Source";
            this.colSource.Name = "colSource";
            // 
            // openLogFile
            // 
            this.openLogFile.FileName = "*.*";
            this.openLogFile.Filter = "YamuraLog v5|*.yl5|YamuraLog|*.ylg|All|*.*";
            this.openLogFile.FilterIndex = 0;
            this.openLogFile.Multiselect = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sessionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // sessionsToolStripMenuItem
            // 
            this.sessionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSessions,
            this.removeSessions});
            this.sessionsToolStripMenuItem.Name = "sessionsToolStripMenuItem";
            this.sessionsToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.sessionsToolStripMenuItem.Text = "Sessions";
            // 
            // addSessions
            // 
            this.addSessions.Name = "addSessions";
            this.addSessions.Size = new System.Drawing.Size(180, 22);
            this.addSessions.Text = "Add Session(s)";
            this.addSessions.Click += new System.EventHandler(this.addSessionMenuItem_Click);
            // 
            // removeSessions
            // 
            this.removeSessions.Name = "removeSessions";
            this.removeSessions.Size = new System.Drawing.Size(180, 22);
            this.removeSessions.Text = "Remove Session(s)";
            this.removeSessions.Click += new System.EventHandler(this.removeSessionsMenuItem_Click);
            // 
            // ManageSessions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridSessions);
            this.Controls.Add(this.menuStrip1);
            this.Name = "ManageSessions";
            this.Text = "Manage Sessions";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSessions)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridSessions;
        private System.Windows.Forms.OpenFileDialog openLogFile;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSession;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStart;
        private System.Windows.Forms.DataGridViewTextBoxColumn colEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn colChannels;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSource;
        private System.Windows.Forms.ToolStripMenuItem sessionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSessions;
        private System.Windows.Forms.ToolStripMenuItem removeSessions;
    }
}