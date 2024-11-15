
namespace WolverineLauncher
{
    partial class CompressionProgress
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
            this.progressBar_Compression = new System.Windows.Forms.ProgressBar();
            this.label_Compression_Title = new System.Windows.Forms.Label();
            this.label_Compression_Description = new System.Windows.Forms.Label();
            this.label_Compression_Progress = new System.Windows.Forms.Label();
            this.pictureBox_Outline = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar_Compression
            // 
            this.progressBar_Compression.BackColor = System.Drawing.Color.Silver;
            this.progressBar_Compression.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.progressBar_Compression.Location = new System.Drawing.Point(23, 96);
            this.progressBar_Compression.Name = "progressBar_Compression";
            this.progressBar_Compression.Size = new System.Drawing.Size(355, 23);
            this.progressBar_Compression.TabIndex = 0;
            // 
            // label_Compression_Title
            // 
            this.label_Compression_Title.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.label_Compression_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_Compression_Title.Location = new System.Drawing.Point(90, 10);
            this.label_Compression_Title.Name = "label_Compression_Title";
            this.label_Compression_Title.Size = new System.Drawing.Size(220, 20);
            this.label_Compression_Title.TabIndex = 26;
            this.label_Compression_Title.Text = "Compression in progress";
            this.label_Compression_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_Compression_Description
            // 
            this.label_Compression_Description.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.label_Compression_Description.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_Compression_Description.Location = new System.Drawing.Point(23, 30);
            this.label_Compression_Description.Name = "label_Compression_Description";
            this.label_Compression_Description.Size = new System.Drawing.Size(355, 63);
            this.label_Compression_Description.TabIndex = 27;
            this.label_Compression_Description.Text = "Games files compression / uncompression is a heavy process that can take up to 8 " +
    "hours to perform...\r\n\r\nDo not close WolverineLauncher before task is complete!";
            // 
            // label_Compression_Progress
            // 
            this.label_Compression_Progress.BackColor = System.Drawing.Color.White;
            this.label_Compression_Progress.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.label_Compression_Progress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_Compression_Progress.ForeColor = System.Drawing.Color.Black;
            this.label_Compression_Progress.Location = new System.Drawing.Point(100, 123);
            this.label_Compression_Progress.Name = "label_Compression_Progress";
            this.label_Compression_Progress.Size = new System.Drawing.Size(200, 16);
            this.label_Compression_Progress.TabIndex = 28;
            this.label_Compression_Progress.Text = "0 / 1000";
            this.label_Compression_Progress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_Compression_Progress.Visible = false;
            // 
            // pictureBox_Outline
            // 
            this.pictureBox_Outline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_Outline.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.pictureBox_Outline.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Outline.Name = "pictureBox_Outline";
            this.pictureBox_Outline.Size = new System.Drawing.Size(400, 145);
            this.pictureBox_Outline.TabIndex = 29;
            this.pictureBox_Outline.TabStop = false;
            // 
            // CompressionProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(400, 145);
            this.Controls.Add(this.label_Compression_Progress);
            this.Controls.Add(this.label_Compression_Description);
            this.Controls.Add(this.label_Compression_Title);
            this.Controls.Add(this.progressBar_Compression);
            this.Controls.Add(this.pictureBox_Outline);
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(400, 145);
            this.MinimumSize = new System.Drawing.Size(400, 145);
            this.Name = "CompressionProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Compression Progress";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CompressionProgress_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar_Compression;
        private System.Windows.Forms.Label label_Compression_Title;
        private System.Windows.Forms.Label label_Compression_Description;
        private System.Windows.Forms.Label label_Compression_Progress;
        private System.Windows.Forms.PictureBox pictureBox_Outline;
    }
}