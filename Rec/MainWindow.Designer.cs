namespace Rec {
    partial class MainWindow {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.openScanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.labelLeftMarker = new System.Windows.Forms.Label();
            this.labelRightMarker = new System.Windows.Forms.Label();
            this.labelAngle = new System.Windows.Forms.Label();
            this.labelMarkersDistance = new System.Windows.Forms.Label();
            this.groupBoxAnalysis = new System.Windows.Forms.GroupBox();
            this.pictureBoxGraph = new System.Windows.Forms.PictureBox();
            this.pictureBoxChecked = new System.Windows.Forms.PictureBox();
            this.listBoxChecked = new System.Windows.Forms.ListBox();
            this.pictureBoxBlank = new System.Windows.Forms.PictureBox();
            this.menuStrip.SuspendLayout();
            this.groupBoxAnalysis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxChecked)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlank)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openScanToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(802, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip";
            // 
            // openScanToolStripMenuItem
            // 
            this.openScanToolStripMenuItem.Name = "openScanToolStripMenuItem";
            this.openScanToolStripMenuItem.Size = new System.Drawing.Size(77, 20);
            this.openScanToolStripMenuItem.Text = "Open form";
            this.openScanToolStripMenuItem.Click += new System.EventHandler(this.openScanToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Images (*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG|Все файлы (*.*)|*.*";
            this.openFileDialog.Title = "Open";
            // 
            // labelLeftMarker
            // 
            this.labelLeftMarker.AutoSize = true;
            this.labelLeftMarker.Location = new System.Drawing.Point(6, 16);
            this.labelLeftMarker.Name = "labelLeftMarker";
            this.labelLeftMarker.Size = new System.Drawing.Size(60, 13);
            this.labelLeftMarker.TabIndex = 3;
            this.labelLeftMarker.Text = "Left marker";
            // 
            // labelRightMarker
            // 
            this.labelRightMarker.AutoSize = true;
            this.labelRightMarker.Location = new System.Drawing.Point(6, 29);
            this.labelRightMarker.Name = "labelRightMarker";
            this.labelRightMarker.Size = new System.Drawing.Size(67, 13);
            this.labelRightMarker.TabIndex = 4;
            this.labelRightMarker.Text = "Right marker";
            // 
            // labelAngle
            // 
            this.labelAngle.AutoSize = true;
            this.labelAngle.Location = new System.Drawing.Point(6, 42);
            this.labelAngle.Name = "labelAngle";
            this.labelAngle.Size = new System.Drawing.Size(47, 13);
            this.labelAngle.TabIndex = 5;
            this.labelAngle.Text = "Rotation";
            // 
            // labelMarkersDistance
            // 
            this.labelMarkersDistance.AutoSize = true;
            this.labelMarkersDistance.Location = new System.Drawing.Point(6, 55);
            this.labelMarkersDistance.Name = "labelMarkersDistance";
            this.labelMarkersDistance.Size = new System.Drawing.Size(133, 13);
            this.labelMarkersDistance.TabIndex = 6;
            this.labelMarkersDistance.Text = "Distance between markers";
            // 
            // groupBoxAnalysis
            // 
            this.groupBoxAnalysis.Controls.Add(this.pictureBoxGraph);
            this.groupBoxAnalysis.Controls.Add(this.pictureBoxChecked);
            this.groupBoxAnalysis.Controls.Add(this.listBoxChecked);
            this.groupBoxAnalysis.Controls.Add(this.labelLeftMarker);
            this.groupBoxAnalysis.Controls.Add(this.labelRightMarker);
            this.groupBoxAnalysis.Controls.Add(this.labelMarkersDistance);
            this.groupBoxAnalysis.Controls.Add(this.labelAngle);
            this.groupBoxAnalysis.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBoxAnalysis.Location = new System.Drawing.Point(483, 24);
            this.groupBoxAnalysis.Name = "groupBoxAnalysis";
            this.groupBoxAnalysis.Size = new System.Drawing.Size(319, 568);
            this.groupBoxAnalysis.TabIndex = 8;
            this.groupBoxAnalysis.TabStop = false;
            this.groupBoxAnalysis.Text = "Analysis";
            // 
            // pictureBoxGraph
            // 
            this.pictureBoxGraph.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxGraph.Location = new System.Drawing.Point(9, 315);
            this.pictureBoxGraph.Name = "pictureBoxGraph";
            this.pictureBoxGraph.Size = new System.Drawing.Size(298, 241);
            this.pictureBoxGraph.TabIndex = 10;
            this.pictureBoxGraph.TabStop = false;
            this.pictureBoxGraph.Click += new System.EventHandler(this.pictureBoxGraph_Click);
            // 
            // pictureBoxChecked
            // 
            this.pictureBoxChecked.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxChecked.Location = new System.Drawing.Point(9, 71);
            this.pictureBoxChecked.Name = "pictureBoxChecked";
            this.pictureBoxChecked.Size = new System.Drawing.Size(212, 238);
            this.pictureBoxChecked.TabIndex = 9;
            this.pictureBoxChecked.TabStop = false;
            this.pictureBoxChecked.Click += new System.EventHandler(this.pictureBoxChecked_Click);
            // 
            // listBoxChecked
            // 
            this.listBoxChecked.FormattingEnabled = true;
            this.listBoxChecked.Location = new System.Drawing.Point(227, 19);
            this.listBoxChecked.Name = "listBoxChecked";
            this.listBoxChecked.Size = new System.Drawing.Size(80, 290);
            this.listBoxChecked.TabIndex = 8;
            // 
            // pictureBoxBlank
            // 
            this.pictureBoxBlank.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxBlank.Location = new System.Drawing.Point(0, 24);
            this.pictureBoxBlank.Name = "pictureBoxBlank";
            this.pictureBoxBlank.Size = new System.Drawing.Size(802, 568);
            this.pictureBoxBlank.TabIndex = 2;
            this.pictureBoxBlank.TabStop = false;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(802, 592);
            this.Controls.Add(this.groupBoxAnalysis);
            this.Controls.Add(this.pictureBoxBlank);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainWindow";
            this.Text = "Test form recognizer";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.groupBoxAnalysis.ResumeLayout(false);
            this.groupBoxAnalysis.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxGraph)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxChecked)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBlank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem openScanToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.PictureBox pictureBoxBlank;
        private System.Windows.Forms.Label labelLeftMarker;
        private System.Windows.Forms.Label labelRightMarker;
        private System.Windows.Forms.Label labelAngle;
        private System.Windows.Forms.Label labelMarkersDistance;
        private System.Windows.Forms.GroupBox groupBoxAnalysis;
        private System.Windows.Forms.ListBox listBoxChecked;
        private System.Windows.Forms.PictureBox pictureBoxChecked;
        private System.Windows.Forms.PictureBox pictureBoxGraph;
    }
}

