namespace YamuraView
{
    partial class YamuraViewMain
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.sessionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addStripChartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTractionCircleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addTrackMapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sessionsToolStripMenuItem,
            this.addStripChartMenuItem,
            this.addTractionCircleMenuItem,
            this.addTrackMapMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(49, 20);
            this.toolStripMenuItem1.Text = "Views";
            // 
            // sessionsToolStripMenuItem
            // 
            this.sessionsToolStripMenuItem.Name = "sessionsToolStripMenuItem";
            this.sessionsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.sessionsToolStripMenuItem.Text = "Manage Sessions";
            this.sessionsToolStripMenuItem.Click += new System.EventHandler(this.addSessionsMenuItem_Click);
            // 
            // addStripChartMenuItem
            // 
            this.addStripChartMenuItem.Name = "addStripChartMenuItem";
            this.addStripChartMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addStripChartMenuItem.Text = "Add Strip Chart";
            this.addStripChartMenuItem.Click += new System.EventHandler(this.addStripChartMenuItem_Click);
            // 
            // addTractionCircleMenuItem
            // 
            this.addTractionCircleMenuItem.Name = "addTractionCircleMenuItem";
            this.addTractionCircleMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addTractionCircleMenuItem.Text = "Add Traction Circle";
            this.addTractionCircleMenuItem.Click += new System.EventHandler(this.addTractionCircleMenuItem_Click);
            // 
            // addTrackMapMenuItem
            // 
            this.addTrackMapMenuItem.Name = "addTrackMapMenuItem";
            this.addTrackMapMenuItem.Size = new System.Drawing.Size(180, 22);
            this.addTrackMapMenuItem.Text = "Add Track Map";
            this.addTrackMapMenuItem.Click += new System.EventHandler(this.addTrackMapMenuItem_Click);
            // 
            // YamuraViewMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "YamuraViewMain";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addStripChartMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTractionCircleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addTrackMapMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sessionsToolStripMenuItem;
    }
}

