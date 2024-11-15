
namespace WolverineLauncher
{
    partial class Hint
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
            this.pictureBox_Outline = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_Outline
            // 
            this.pictureBox_Outline.BackgroundImage = global::WolverineLauncher.Properties.Resources.Hint_Missing;
            this.pictureBox_Outline.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_Outline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_Outline.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Outline.Name = "pictureBox_Outline";
            this.pictureBox_Outline.Size = new System.Drawing.Size(560, 315);
            this.pictureBox_Outline.TabIndex = 0;
            this.pictureBox_Outline.TabStop = false;
            // 
            // Hint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(560, 315);
            this.Controls.Add(this.pictureBox_Outline);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(32, 32);
            this.MaximumSize = new System.Drawing.Size(560, 315);
            this.MinimumSize = new System.Drawing.Size(560, 315);
            this.Name = "Hint";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Marvel\'s Wolverine - Hint";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_Outline;
    }
}