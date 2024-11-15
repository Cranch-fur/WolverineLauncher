#define CHECK_GAME_SAFEPATH
#define CHECK_GAME_BASEFILES
#define CHECK_GAME_SIZE

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WolverineLauncher
{
    public partial class Launcher : Form
    {
        private static Settings _settings = new Settings();


        private static string[] legacyFiles = new string[]
        {
        };
        private static Tuple<string, string>[] moveFiles = new Tuple<string, string>[] // {Source Path} {Destination Path}
        {
        };
        private static string[] legacyDirectories = new string[]
        {
        };




        private void InitializeForm()
        {
            this.Icon = Properties.Resources.Icon;
            label_Version.Text = GetUserDefinedVersion();

#if DEBUG
            label_DebugMode.Visible = true;
            button_DebugCMDSettings.Visible = true;
            button_DebugStartupArguments.Visible = true;
#endif
            
            if (Overrides._unsafeMode == true)
            {
                label_UnsafeMode.Visible = true;
            }
        }
        private void InitializeFonts()
        {
            labelButton_StartGame.Parent = pictureBox_Outline;
            labelButton_StartGame.BackColor = Color.Transparent;

            labelButton_Settings.Parent = pictureBox_Outline;
            labelButton_Settings.BackColor = Color.Transparent;

            labelButton_Exit.Parent = pictureBox_Outline;
            labelButton_Exit.BackColor = Color.Transparent;

            label_Version.Parent = pictureBox_Outline;
            label_Version.BackColor = Color.Transparent;

#if DEBUG
            label_DebugMode.Parent = pictureBox_Outline;
            label_DebugMode.BackColor = Color.Transparent;
#endif

            label_UnsafeMode.Parent = pictureBox_Outline;

            checkBox_ShowLauncher.Parent = pictureBox_Outline;
            checkBox_ShowLauncher.BackColor = Color.Transparent;
        }




        private string GetUserDefinedVersion()
        {
            if (File.Exists(Constants._versionPath))
            {
                try
                {
                    return File.ReadAllText(Constants._versionPath);
                }
                catch
                {
                    return "UNKNOWN";
                }
            }

            return "MISSING \"version.txt\"";
        }

#if CHECK_GAME_SAFEPATH
        private Tuple<bool, string> GetStringIsSafeForPath(string input)
        {
            // REGEX: a-z, A-Z, 0-9, "/", "\", ":", "!", ".", "_", "-", "(", ")".

            string problematicRegions = null;
            bool isSafe = Regex.IsMatch(input, @"^[a-zA-Z0-9 /\\:!.\-_()]*$");

            if (isSafe == false)
                problematicRegions = Regex.Replace(input, @"[^a-zA-Z0-9 /\\:!.\-_()]", "#");

            return Tuple.Create(isSafe, problematicRegions);
        }
#endif

#if CHECK_GAME_BASEFILES
        private static bool GetBaseFilesExist()
        {
            bool baseFilesExist = true;
            string[] baseFiles = { "igFoundation.dll", "DDLFoundation.dll" };
            string[] baseDirectories = { "d" };

            foreach (string fileName in baseFiles)
            {
                if (File.Exists(Path.Combine(Constants._workspaceDirectory, fileName)) == false)
                    baseFilesExist = false;
            }
            foreach (string directoryName in baseDirectories)
            {
                if (Directory.Exists(Path.Combine(Constants._workspaceDirectory, directoryName)) == false)
                    baseFilesExist = false;
            }

            return baseFilesExist;
        }
#endif

#if CHECK_GAME_SIZE
        private static Tuple<double, double> GetDirectorySize(string path)
        {
            if (Directory.Exists(path) == false)
                return Tuple.Create(0.0, 0.0);

            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            long size = 0;

            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                size += fileInfo.Length;
            }

            double sizeMB = Convert.ToDouble(size) / (1024.0 * 1024.0);
            double sizeGB = sizeMB / 1024.0;

            return Tuple.Create(sizeMB, sizeGB);
        }
#endif




        private static List<string> GetGameStartupArguments()
        {
            List<String> startupArguments = new List<string>(_settings._gameStartupArguments);

            if (string.IsNullOrEmpty(_settings._customGameStartupArguments) == false)
                startupArguments.Add(_settings._customGameStartupArguments);

            startupArguments.RemoveAll(arg => arg == string.Empty); // Remove all occurencies of empty entry from the list
            return startupArguments;
        }




        private static void DestroyLegacyFiles()
        {
            foreach (string file in legacyFiles)
            {
                if (File.Exists(file))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }

            foreach (Tuple<string, string> moveFile in moveFiles)
            {
                if (File.Exists(moveFile.Item1))
                {
                    try
                    {
                        if (File.Exists(moveFile.Item2) == false)
                        {
                            File.Move(moveFile.Item1, moveFile.Item2);
                        }
                        else
                        {
                            File.Delete(moveFile.Item1);
                        }
                    }
                    catch { }
                    
                }
            }
        }
        private static void DestroyLegacyDirectories()
        {
            foreach (string directory in legacyDirectories)
            {
                if (Directory.Exists(directory))
                {
                    try
                    {
                        Directory.Delete(directory, true);
                    }
                    catch { }
                }
            }
        }



        private void Launcher_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = _settings.GetCompressionProgressRunning();
        }
        public Launcher()
        {
            InitializeComponent();
            InitializeForm();
            InitializeFonts();

            DestroyLegacyFiles();
            DestroyLegacyDirectories();
        }
        private void Launcher_Load(object sender, EventArgs e)
        {
            if (Overrides._noLauncher == true)
            {
                while (_settings._settingsInitialized == false)
                {
                    continue;
                }

                StartGame();
            }
        }



        private void StartGame()
        {
            if (_settings.GetCompressionProgressRunning() == true)
            {
                return;
            }


#if CHECK_GAME_SAFEPATH
            if (Overrides._unsafeMode == false)
            {
                Tuple<bool, string> isPathSafe = GetStringIsSafeForPath(Constants._currentDirectory);
                if (isPathSafe.Item1 == false)
                {
                    MessageBox.Show($"Wolverine path should only contain English characters!\n\"{isPathSafe.Item2}\"\n\nProblematic regions has been marked with \"#\" symbols, consider removing OR overwriting them before playing.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
#endif


#if CHECK_GAME_BASEFILES
            if (Overrides._unsafeMode == false)
            {
                if (GetBaseFilesExist() == false)
                {
                    MessageBox.Show($"Wolverine isn't installed properly OR game files are corrupt!\n\n\"igFoundation.dll\" & \"DDLFoundation.dll\" base files are missing. Was game even downloaded / installed in first place?", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
#endif


#if CHECK_GAME_SIZE
            if (Overrides._unsafeMode == false)
            {
                Tuple<double, double> workspaceSize = GetDirectorySize(Path.Combine(Constants._workspaceDirectory, "d"));
                if (workspaceSize.Item1 < 66560.0)
                {
                    MessageBox.Show($"Wolverine isn't installed properly OR game files are corrupt!\n\n\"...\\workspace\\d\" folder size is {Math.Round(workspaceSize.Item2, 2)}GB / 65GB+ expected. Was game even downloaded / installed in first place?", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
#endif


            string gameExecutablePath = _settings._gameAnselExecutable ? Constants._anselGameExecutablePath : Constants._gameExecutablePath;

            if (File.Exists(gameExecutablePath))
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = gameExecutablePath;
                    process.StartInfo.WorkingDirectory = Constants._workspaceDirectory;
                    process.StartInfo.Arguments = string.Join(" ", GetGameStartupArguments());
                    if (_settings._gameAdministratorMode)
                        process.StartInfo.Verb = "runas";

                    try
                    {
                        process.Start();
                    }
                    catch
                    {
                        MessageBox.Show($"WolverineLauncher failed to start \"{gameExecutablePath}\"!\n\nMake sure that file exist in first place and wasn't affected by 3rd party tools such as Anti-Virus Software.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                Application.Exit();
                return;
            }
            else
            {
                MessageBox.Show($"WolverineLauncher failed to access \"{gameExecutablePath}\"!\n\nMake sure that file exist in first place and wasn't affected by 3rd party tools such as Anti-Virus Software.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void labelButton_StartGame_MouseEnter(object sender, EventArgs e) => labelButton_StartGame.ForeColor = Color.FromArgb(190, 38, 38);
        private void labelButton_StartGame_MouseLeave(object sender, EventArgs e) => labelButton_StartGame.ForeColor = Color.Transparent;
        private void labelButton_StartGame_MouseClick(object sender, MouseEventArgs e) => StartGame();

        private void labelButton_Settings_MouseEnter(object sender, EventArgs e) => labelButton_Settings.ForeColor = Color.FromArgb(190, 38, 38);
        private void labelButton_Settings_MouseLeave(object sender, EventArgs e) => labelButton_Settings.ForeColor = Color.Transparent;
        private void labelButton_Settings_MouseClick(object sender, MouseEventArgs e)
        {
            _settings.ShowDialog(this);
        }

        private void labelButton_Exit_MouseEnter(object sender, EventArgs e) => labelButton_Exit.ForeColor = Color.FromArgb(190, 38, 38);
        private void labelButton_Exit_MouseLeave(object sender, EventArgs e) => labelButton_Exit.ForeColor = Color.Transparent;
        private void labelButton_Exit_MouseClick(object sender, MouseEventArgs e) => Application.Exit();

        private void button_DebugStartupArguments_MouseClick(object sender, MouseEventArgs e)
        {
#if DEBUG
            if (textBox_DebugStartupArguments.Visible)
            {
                textBox_DebugStartupArguments.Visible = false;
            }
            else
            {
                textBox_DebugStartupArguments.Text = string.Empty;
                textBox_DebugStartupArguments.Visible = true;

                foreach (string argument in GetGameStartupArguments())
                {
                    textBox_DebugStartupArguments.Text = textBox_DebugStartupArguments.Text + $"{argument}{Environment.NewLine}";
                }
            }
#endif
        }

        private void button_DebugCMDSettings_MouseClick(object sender, MouseEventArgs e)
        {
#if DEBUG
            if (textBox_DebugCMDSettings.Visible)
            {
                textBox_DebugCMDSettings.Visible = false;
            }
            else
            {
                textBox_DebugCMDSettings.Text = string.Empty;
                textBox_DebugCMDSettings.Visible = true;

                string CMD = File.ReadAllText(Path.Combine(Constants._currentDirectory, "workspace", "cmd.txt"));
                textBox_DebugCMDSettings.Text = CMD;
            }
#endif
        }

        private void checkBox_ShowLauncher_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_ShowLauncher.Checked == false)
            {
                MessageBox.Show($"WolverineLauncher can be disabled in order to not interact with it on every game startup. To make it work this way, launch it with following startup argument:\n\n-nolauncher", "Disable Launcher", MessageBoxButtons.OK, MessageBoxIcon.Information);
                checkBox_ShowLauncher.Checked = true;
            }
        }
    }
}
