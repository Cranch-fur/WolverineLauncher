
namespace WolverineLauncher
{
    partial class Launcher
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelButton_Exit = new System.Windows.Forms.Label();
            this.label_Version = new System.Windows.Forms.Label();
            this.pictureBox_Outline = new System.Windows.Forms.PictureBox();
            this.labelButton_Settings = new System.Windows.Forms.Label();
            this.labelButton_StartGame = new System.Windows.Forms.Label();
            this.button_DebugStartupArguments = new System.Windows.Forms.Button();
            this.label_DebugMode = new System.Windows.Forms.Label();
            this.textBox_DebugStartupArguments = new System.Windows.Forms.TextBox();
            this.button_DebugCMDSettings = new System.Windows.Forms.Button();
            this.textBox_DebugCMDSettings = new System.Windows.Forms.TextBox();
            this.label_UnsafeMode = new System.Windows.Forms.Label();
            this.checkBox_ShowLauncher = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).BeginInit();
            this.SuspendLayout();
            // 
            // labelButton_Exit
            // 
            this.labelButton_Exit.BackColor = System.Drawing.Color.Black;
            this.labelButton_Exit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelButton_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelButton_Exit.ForeColor = System.Drawing.Color.White;
            this.labelButton_Exit.Location = new System.Drawing.Point(28, 385);
            this.labelButton_Exit.Name = "labelButton_Exit";
            this.labelButton_Exit.Size = new System.Drawing.Size(128, 24);
            this.labelButton_Exit.TabIndex = 1;
            this.labelButton_Exit.Text = "Quit";
            this.labelButton_Exit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelButton_Exit.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelButton_Exit_MouseClick);
            this.labelButton_Exit.MouseEnter += new System.EventHandler(this.labelButton_Exit_MouseEnter);
            this.labelButton_Exit.MouseLeave += new System.EventHandler(this.labelButton_Exit_MouseLeave);
            // 
            // label_Version
            // 
            this.label_Version.BackColor = System.Drawing.Color.Black;
            this.label_Version.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_Version.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_Version.ForeColor = System.Drawing.Color.White;
            this.label_Version.Location = new System.Drawing.Point(527, 419);
            this.label_Version.Name = "label_Version";
            this.label_Version.Size = new System.Drawing.Size(256, 24);
            this.label_Version.TabIndex = 2;
            this.label_Version.Text = "Milestone 10";
            this.label_Version.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pictureBox_Outline
            // 
            this.pictureBox_Outline.BackgroundImage = global::WolverineLauncher.Properties.Resources.Img_Launcher;
            this.pictureBox_Outline.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox_Outline.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox_Outline.Location = new System.Drawing.Point(0, 0);
            this.pictureBox_Outline.Name = "pictureBox_Outline";
            this.pictureBox_Outline.Size = new System.Drawing.Size(790, 450);
            this.pictureBox_Outline.TabIndex = 0;
            this.pictureBox_Outline.TabStop = false;
            // 
            // labelButton_Settings
            // 
            this.labelButton_Settings.BackColor = System.Drawing.Color.Black;
            this.labelButton_Settings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelButton_Settings.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelButton_Settings.ForeColor = System.Drawing.Color.White;
            this.labelButton_Settings.Location = new System.Drawing.Point(28, 345);
            this.labelButton_Settings.Name = "labelButton_Settings";
            this.labelButton_Settings.Size = new System.Drawing.Size(128, 24);
            this.labelButton_Settings.TabIndex = 3;
            this.labelButton_Settings.Text = "Settings";
            this.labelButton_Settings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelButton_Settings.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelButton_Settings_MouseClick);
            this.labelButton_Settings.MouseEnter += new System.EventHandler(this.labelButton_Settings_MouseEnter);
            this.labelButton_Settings.MouseLeave += new System.EventHandler(this.labelButton_Settings_MouseLeave);
            // 
            // labelButton_StartGame
            // 
            this.labelButton_StartGame.BackColor = System.Drawing.Color.Black;
            this.labelButton_StartGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelButton_StartGame.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelButton_StartGame.ForeColor = System.Drawing.Color.White;
            this.labelButton_StartGame.Location = new System.Drawing.Point(28, 321);
            this.labelButton_StartGame.Name = "labelButton_StartGame";
            this.labelButton_StartGame.Size = new System.Drawing.Size(128, 24);
            this.labelButton_StartGame.TabIndex = 4;
            this.labelButton_StartGame.Text = "Start Game";
            this.labelButton_StartGame.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelButton_StartGame.MouseClick += new System.Windows.Forms.MouseEventHandler(this.labelButton_StartGame_MouseClick);
            this.labelButton_StartGame.MouseEnter += new System.EventHandler(this.labelButton_StartGame_MouseEnter);
            this.labelButton_StartGame.MouseLeave += new System.EventHandler(this.labelButton_StartGame_MouseLeave);
            // 
            // button_DebugStartupArguments
            // 
            this.button_DebugStartupArguments.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button_DebugStartupArguments.FlatAppearance.BorderSize = 0;
            this.button_DebugStartupArguments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_DebugStartupArguments.Location = new System.Drawing.Point(579, 12);
            this.button_DebugStartupArguments.Name = "button_DebugStartupArguments";
            this.button_DebugStartupArguments.Size = new System.Drawing.Size(200, 24);
            this.button_DebugStartupArguments.TabIndex = 5;
            this.button_DebugStartupArguments.Text = "Get Startup Arguments";
            this.button_DebugStartupArguments.UseVisualStyleBackColor = false;
            this.button_DebugStartupArguments.Visible = false;
            this.button_DebugStartupArguments.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button_DebugStartupArguments_MouseClick);
            // 
            // label_DebugMode
            // 
            this.label_DebugMode.BackColor = System.Drawing.Color.Black;
            this.label_DebugMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_DebugMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_DebugMode.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label_DebugMode.Location = new System.Drawing.Point(12, 12);
            this.label_DebugMode.Name = "label_DebugMode";
            this.label_DebugMode.Size = new System.Drawing.Size(144, 24);
            this.label_DebugMode.TabIndex = 6;
            this.label_DebugMode.Text = "DEBUG MODE";
            this.label_DebugMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label_DebugMode.Visible = false;
            // 
            // textBox_DebugStartupArguments
            // 
            this.textBox_DebugStartupArguments.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textBox_DebugStartupArguments.Location = new System.Drawing.Point(579, 42);
            this.textBox_DebugStartupArguments.Multiline = true;
            this.textBox_DebugStartupArguments.Name = "textBox_DebugStartupArguments";
            this.textBox_DebugStartupArguments.ReadOnly = true;
            this.textBox_DebugStartupArguments.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_DebugStartupArguments.Size = new System.Drawing.Size(200, 128);
            this.textBox_DebugStartupArguments.TabIndex = 7;
            this.textBox_DebugStartupArguments.Visible = false;
            // 
            // button_DebugCMDSettings
            // 
            this.button_DebugCMDSettings.BackColor = System.Drawing.Color.WhiteSmoke;
            this.button_DebugCMDSettings.FlatAppearance.BorderSize = 0;
            this.button_DebugCMDSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_DebugCMDSettings.Location = new System.Drawing.Point(315, 12);
            this.button_DebugCMDSettings.Name = "button_DebugCMDSettings";
            this.button_DebugCMDSettings.Size = new System.Drawing.Size(256, 24);
            this.button_DebugCMDSettings.TabIndex = 8;
            this.button_DebugCMDSettings.Text = "Get CMD Settings";
            this.button_DebugCMDSettings.UseVisualStyleBackColor = false;
            this.button_DebugCMDSettings.Visible = false;
            this.button_DebugCMDSettings.MouseClick += new System.Windows.Forms.MouseEventHandler(this.button_DebugCMDSettings_MouseClick);
            // 
            // textBox_DebugCMDSettings
            // 
            this.textBox_DebugCMDSettings.BackColor = System.Drawing.Color.WhiteSmoke;
            this.textBox_DebugCMDSettings.Location = new System.Drawing.Point(315, 42);
            this.textBox_DebugCMDSettings.Multiline = true;
            this.textBox_DebugCMDSettings.Name = "textBox_DebugCMDSettings";
            this.textBox_DebugCMDSettings.ReadOnly = true;
            this.textBox_DebugCMDSettings.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_DebugCMDSettings.Size = new System.Drawing.Size(256, 128);
            this.textBox_DebugCMDSettings.TabIndex = 9;
            this.textBox_DebugCMDSettings.Visible = false;
            // 
            // label_UnsafeMode
            // 
            this.label_UnsafeMode.BackColor = System.Drawing.Color.Black;
            this.label_UnsafeMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_UnsafeMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label_UnsafeMode.ForeColor = System.Drawing.Color.Red;
            this.label_UnsafeMode.Location = new System.Drawing.Point(323, 419);
            this.label_UnsafeMode.Name = "label_UnsafeMode";
            this.label_UnsafeMode.Size = new System.Drawing.Size(144, 24);
            this.label_UnsafeMode.TabIndex = 10;
            this.label_UnsafeMode.Text = "UNSAFE MODE";
            this.label_UnsafeMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_UnsafeMode.Visible = false;
            // 
            // checkBox_ShowLauncher
            // 
            this.checkBox_ShowLauncher.BackColor = System.Drawing.Color.Black;
            this.checkBox_ShowLauncher.Checked = true;
            this.checkBox_ShowLauncher.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_ShowLauncher.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox_ShowLauncher.ForeColor = System.Drawing.Color.White;
            this.checkBox_ShowLauncher.Location = new System.Drawing.Point(667, 400);
            this.checkBox_ShowLauncher.Name = "checkBox_ShowLauncher";
            this.checkBox_ShowLauncher.Size = new System.Drawing.Size(118, 20);
            this.checkBox_ShowLauncher.TabIndex = 11;
            this.checkBox_ShowLauncher.Text = "Show Launcher";
            this.checkBox_ShowLauncher.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBox_ShowLauncher.UseVisualStyleBackColor = false;
            this.checkBox_ShowLauncher.CheckedChanged += new System.EventHandler(this.checkBox_ShowLauncher_CheckedChanged);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(790, 450);
            this.Controls.Add(this.label_UnsafeMode);
            this.Controls.Add(this.textBox_DebugCMDSettings);
            this.Controls.Add(this.button_DebugCMDSettings);
            this.Controls.Add(this.textBox_DebugStartupArguments);
            this.Controls.Add(this.label_DebugMode);
            this.Controls.Add(this.button_DebugStartupArguments);
            this.Controls.Add(this.labelButton_Exit);
            this.Controls.Add(this.labelButton_Settings);
            this.Controls.Add(this.labelButton_StartGame);
            this.Controls.Add(this.label_Version);
            this.Controls.Add(this.checkBox_ShowLauncher);
            this.Controls.Add(this.pictureBox_Outline);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(790, 450);
            this.MinimumSize = new System.Drawing.Size(790, 450);
            this.Name = "Launcher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Marvel\'s Wolverine Launcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Launcher_FormClosing);
            this.Load += new System.EventHandler(this.Launcher_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Outline)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_Outline;
        private System.Windows.Forms.Label labelButton_Exit;
        private System.Windows.Forms.Label label_Version;
        private System.Windows.Forms.Label labelButton_Settings;
        private System.Windows.Forms.Label labelButton_StartGame;
        private System.Windows.Forms.Button button_DebugStartupArguments;
        private System.Windows.Forms.Label label_DebugMode;
        private System.Windows.Forms.TextBox textBox_DebugStartupArguments;
        private System.Windows.Forms.Button button_DebugCMDSettings;
        private System.Windows.Forms.TextBox textBox_DebugCMDSettings;
        private System.Windows.Forms.Label label_UnsafeMode;
        private System.Windows.Forms.CheckBox checkBox_ShowLauncher;
    }
}

