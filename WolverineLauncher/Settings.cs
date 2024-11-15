using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WolverineLauncher
{
    public partial class Settings : Form
    {
        private static Hint _hint = new Hint();
        private static DebugControlsHint _debugControlsHint = new DebugControlsHint();
        private static CompressionProgress _compressionProgress = new CompressionProgress();

        private static ToolTip toolTip_Info = new ToolTip
        {
            ToolTipIcon = ToolTipIcon.Info,
            ToolTipTitle = "Information",
            IsBalloon = false
        };
        private static ToolTip toolTip_Warning = new ToolTip
        {
            ToolTipIcon = ToolTipIcon.Warning,
            ToolTipTitle = "Warning",
            IsBalloon = false
        };

        private List<string> compressionFilesList = new List<string> // Filled with ListCompressionFiles()
        {
        };

        private static readonly S_CustomRenderAPI _dxvk = new S_CustomRenderAPI
        {
            libraryPath = Path.Combine(Constants._workspaceDirectory, "dxgi.dll"),
            bridgePath = Path.Combine(Constants._workspaceDirectory, "d3d11.dll")
        };
        private static readonly S_CustomRenderAPI _resorep = new S_CustomRenderAPI
        {
            libraryPath = Path.Combine(Constants._workspaceDirectory, "ori_d3d11.dll"),
            bridgePath = Path.Combine(Constants._workspaceDirectory, "d3d11.dll")
        };
        private static readonly S_CustomRenderAPI_Reshade _reshade = new S_CustomRenderAPI_Reshade
        {
            libraryPath = Path.Combine(Constants._workspaceDirectory, "dxgi.dll"),
            configurationPath = Path.Combine(Constants._workspaceDirectory, "ReShade.ini"),
            shadersDirectoryPath = Path.Combine(Constants._workspaceDirectory, "reshade-shaders")
        };

        private static readonly S_Experiment _experiment_DiscordRPC = new S_Experiment
        {
            path = Path.Combine(Constants._pluginsDirectory, "experiment_discordRPC.asi"),
            disabledPath = Path.Combine(Constants._pluginsDirectory, "experiment_discordRPC.asi_disabled")
        };
        private static readonly S_Experiment _experiment_NoCrashWindow = new S_Experiment
        {
            path = Path.Combine(Constants._pluginsDirectory, "experiment_nocrashwindow.asi"),
            disabledPath = Path.Combine(Constants._pluginsDirectory, "experiment_nocrashwindow.asi_disabled")
        };

        public List<string> _gameStartupArguments = new List<string> { }; // Required for game functioning arguments
        public string _customGameStartupArguments = string.Empty; // Specified by user set of arguments, can be empty

        public bool _gameAdministratorMode = false; // Determines if game need to be launched with Administrator rights (Windows)
        public bool _gameAnselExecutable = false; // Determines if game need to be launched with named as fully supported title executable

        public bool _settingsInitialized = false; // Equals TRUE only after InitializeSettings() function was completed

        private enum E_LanguageIndex
        {
            // {Insomniac Language Code} = {Value} || {Description}
            us = 0,  // English (United States)
            es = 1,  // Spanish
            la = 2,  // Spanish (Latin America)
            cs = 3,  // Chinese (Simplified)
            ct = 4,  // Chinese (Traditional)
            ar = 5,  // Arabic
            pt = 6,  // Portuguese
            br = 7,  // Portuguese (Brazil)
            ru = 8,  // Russian
            jp = 9,  // Japanese
            de = 10, // German
            fr = 11, // French
            fc = 12, // French (Canada)
            kr = 13, // Korean
            it = 14, // Italian
            nl = 15, // Dutch
            se = 16, // Swedish
            pl = 17, // Polish
            no = 18, // Norwegian
            dk = 19, // Danish
            fi = 20, // Finnish
            cz = 21, // Czech
            hu = 22, // Hungarian
            el = 23, // Greek
        }
        private enum E_RenderingAPI
        {
            D3D11,
            VULKAN,
            RESOREP, // D3D11 based texture modding API. Relies on "d3d11.dll" & "ori_d3d11.dll"
            RESHADE, // D3D11 based rendering pipeline modding API. Relies on "dxgi.dll", "ReShade.ini", "reshade-shaders" (folder)
            UNKNOWN
        }
        private enum E_ExperimentState
        {
            DISABLED,
            ENABLED,
            UNKNOWN
        }
        private enum E_HintState
        {
            HIDDEN,
            VISIBLE
        }
        private enum E_OS
        {
            WINDOWS,
            LINUX
        }
        private enum E_DiskType
        {
            HDD,
            SSD
        }
        public struct S_CustomRenderAPI
        {
            public string libraryPath;
            public string bridgePath;

            public S_CustomRenderAPI(string libraryPath, string bridgePath)
            {
                this.libraryPath = libraryPath;
                this.bridgePath = bridgePath;
            }
        }
        public struct S_CustomRenderAPI_Reshade
        {
            public string libraryPath;
            public string configurationPath;
            public string shadersDirectoryPath;

            public S_CustomRenderAPI_Reshade(string libraryPath, string configurationPath, string shadersDirectoryPath)
            {
                this.libraryPath = libraryPath;
                this.configurationPath = configurationPath;
                this.shadersDirectoryPath = shadersDirectoryPath;
            }
        }
        public struct S_Experiment
        {
            public string path;
            public string disabledPath;

            public S_Experiment(string path, string disabledPath)
            {
                this.path = path;
                this.disabledPath = disabledPath;
            }
        }



        private void UpdateHintState(E_HintState newState, Image newImage = null)
        {
            switch (newState)
            {
                case E_HintState.HIDDEN:
                    _hint.Hide();
                    break;

                case E_HintState.VISIBLE:
                    _hint.UpdateImage(newImage);
                    _hint.Show();
                    break;
            }
        }
        private void UpdateRegistry(string key, string value, bool force = false)
        {
            if (_settingsInitialized || force)
            {
                WinReg.SetData_SZ(key, value);
            }
        }
        private void UpdateStartupArguments(string argument, string newValue)
        {
            bool containsArgument = false;
            int argumentIndex = 0;
            int length = _gameStartupArguments.Count;
            for (int i = 0; i < length; i++)
            {
                string currentArgument = _gameStartupArguments[i];
                if (currentArgument.StartsWith(argument))
                {
                    containsArgument = true;
                    argumentIndex = i;
                }
            }

            if (containsArgument)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    _gameStartupArguments.RemoveAt(argumentIndex);
                }
                else
                {
                    _gameStartupArguments[argumentIndex] = newValue;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }
                else
                {
                    _gameStartupArguments.Add(newValue);
                }
            }
        }
        private void UpdatePreciseStartupArguments(string argument, string newValue)
        {
            bool containsArgument = false;
            int argumentIndex = 0;
            int length = _gameStartupArguments.Count;
            for (int i = 0; i < length; i++)
            {
                string currentArgument = _gameStartupArguments[i];
                if (currentArgument.Equals(argument))
                {
                    containsArgument = true;
                    argumentIndex = i;
                }
            }

            if (containsArgument)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    _gameStartupArguments.RemoveAt(argumentIndex);
                }
                else
                {
                    _gameStartupArguments[argumentIndex] = newValue;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }
                else
                {
                    _gameStartupArguments.Add(newValue);
                }
            }
        }



        public bool CheckSettingsFile()
        {
            if (File.Exists(Constants._settingsPath))
            {
                try
                {
                    FileAttributes settingsAttributes = File.GetAttributes(Constants._settingsPath);
                    if (settingsAttributes.HasFlag(FileAttributes.ReadOnly))
                    {
                        File.SetAttributes(Constants._settingsPath, settingsAttributes & ~FileAttributes.ReadOnly);
                    }

                    byte[] settingsBytes = File.ReadAllBytes(Constants._settingsPath); // We need to verify that file can be readen;
                    File.WriteAllBytes(Constants._settingsPath, settingsBytes); // We need to verify that file can be written
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private void UpdateSettings(string argument, string newValue)
        {
            List<string> settings = new List<string>(File.ReadAllLines(Constants._settingsPath));

            bool containsArgument = false;
            int argumentIndex = 0;
            int length = settings.Count;
            for (int i = 0; i < length; i++)
            {
                string currentArgument = settings[i];
                if (currentArgument.StartsWith(argument))
                {
                    containsArgument = true;
                    argumentIndex = i;
                }
            }

            if (containsArgument)
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    settings.RemoveAt(argumentIndex);
                }
                else
                {
                    settings[argumentIndex] = newValue;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(newValue))
                {
                    return;
                }
                else
                {
                    settings.Add(newValue);
                }
            }

            using (StreamWriter streamWriter = new StreamWriter(Constants._settingsPath, false, Encoding.GetEncoding("iso-8859-1")))
            {
                int linesCount = settings.Count;
                for (int i = 0; i < linesCount - 1; i++)
                {
                    streamWriter.WriteLine(settings[i]);
                }

                streamWriter.Write(settings[linesCount - 1]);
            }
        }
        private E_RenderingAPI GetRenderingAPI()
        {
            if (Directory.Exists(Constants._workspaceDirectory))
            {
                if (File.Exists(_reshade.libraryPath) && File.Exists(_reshade.configurationPath) && Directory.Exists(_reshade.shadersDirectoryPath)) // RESHADE
                {
                    if (File.Exists(_dxvk.bridgePath) || File.Exists(_resorep.bridgePath))
                    {
                        File.Delete(_dxvk.bridgePath);
                    }

                    return E_RenderingAPI.RESHADE;
                }
                else if (File.Exists(_dxvk.bridgePath) || File.Exists(_resorep.bridgePath)) // RESOREP & DXVK (VULKAN)
                {
                    byte[] dxvkBridge = Properties.Resources.dxvk_bridge;
                    byte[] localBridge = File.ReadAllBytes(_dxvk.bridgePath);
                    bool isDxvkBridge = localBridge.SequenceEqual(dxvkBridge);

                    if (File.Exists(_resorep.libraryPath) && isDxvkBridge == false)
                    {
                        return E_RenderingAPI.RESOREP;
                    }
                    else if (File.Exists(_dxvk.libraryPath) && isDxvkBridge == true)
                    {
                        return E_RenderingAPI.VULKAN;
                    }
                    else if (File.Exists(_dxvk.libraryPath) == false || File.Exists(_resorep.libraryPath) == false)
                    {
                        return E_RenderingAPI.D3D11;
                    }
                    else
                    {
                        return E_RenderingAPI.UNKNOWN;
                    }
                }
                else
                {
                    return E_RenderingAPI.D3D11;
                }
            }
            else
            {
                return E_RenderingAPI.UNKNOWN;
            }
        }
        private bool SetRenderingAPI(E_RenderingAPI renderingAPI)
        {
            switch (renderingAPI)
            {
                case E_RenderingAPI.D3D11:
                    try
                    {
                        if (File.Exists(_dxvk.libraryPath))
                            File.Delete(_dxvk.libraryPath);

                        if (File.Exists(_dxvk.bridgePath))
                            File.Delete(_dxvk.bridgePath);

                        return true;
                    }
                    catch
                    {
                        MessageBox.Show($"WolverineLauncher failed to delete \"{_dxvk.libraryPath}\" & \"{_dxvk.bridgePath}\" files!\n\nMake sure that files aren't currently in use and doesn't have \"Read Only\" property assigned to them.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }


                case E_RenderingAPI.VULKAN:
                    try
                    {
                        if (File.Exists(_dxvk.libraryPath))
                            File.Delete(_dxvk.libraryPath);

                        byte[] libraryBytes = Properties.Resources.dxvk_library;
                        if (libraryBytes != null && libraryBytes.Length > 0)
                        {
                            using (MemoryStream libraryMemoryStream = new MemoryStream(libraryBytes))
                            {
                                string destinationPath = Path.Combine(Constants._workspaceDirectory, "dxgi.dll");
                                File.WriteAllBytes(destinationPath, libraryBytes);
                            }
                        }




                        if (File.Exists(_dxvk.bridgePath))
                            File.Delete(_dxvk.bridgePath);

                        byte[] bridgeBytes = Properties.Resources.dxvk_bridge;
                        if (bridgeBytes != null && bridgeBytes.Length > 0)
                        {
                            using (MemoryStream bridgeMemoryStream = new MemoryStream(bridgeBytes))
                            {
                                string destinationPath = Path.Combine(Constants._workspaceDirectory, "d3d11.dll");
                                File.WriteAllBytes(destinationPath, bridgeBytes);
                            }
                        }

                        return true;
                    }
                    catch
                    {
                        MessageBox.Show($"WolverineLauncher failed to write \"{_dxvk.libraryPath}\" & \"{_dxvk.bridgePath}\" files!", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }


                default:
                    MessageBox.Show($"WolverineLauncher can't set rendering API of type \"{renderingAPI}\"", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
            }
        }
        private E_ExperimentState GetExperimentState(S_Experiment experiment)
        {
            if (File.Exists(experiment.path))
                return E_ExperimentState.ENABLED;

            else if (File.Exists(experiment.disabledPath))
                return E_ExperimentState.DISABLED;

            else
                return E_ExperimentState.UNKNOWN;
        }
        private bool SetExperimentState(S_Experiment experiment, E_ExperimentState experimentState)
        {
            switch (experimentState)
            {
                case E_ExperimentState.DISABLED:
                    if (File.Exists(experiment.path))
                    {
                        try
                        {
                            if (File.Exists(experiment.disabledPath))
                                File.Delete(experiment.disabledPath);

                            File.Move(experiment.path, experiment.disabledPath);
                            return true;
                        }
                        catch
                        {
                            MessageBox.Show($"WolverineLauncher failed to access \"{experiment.path}\" file!\n\nMake sure that file exist in first place and there's no \"Read Only\" property assigned to it.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    return false;

                case E_ExperimentState.ENABLED:
                    if (File.Exists(experiment.disabledPath))
                    {
                        try
                        {
                            if (File.Exists(experiment.path))
                                File.Delete(experiment.path);

                            File.Move(experiment.disabledPath, experiment.path);
                            return true;
                        }
                        catch
                        {
                            MessageBox.Show($"WolverineLauncher failed to access \"{experiment.disabledPath}\" file!\n\nMake sure that file exist in first place and there's no \"Read Only\" property assigned to it.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }

                    return false;

                default:
                    MessageBox.Show($"WolverineLauncher can't set experiment state of type \"{experimentState}\"", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
            }
        }



        private bool ListCompressionFiles(bool forceNewSearch = false)
        {
            if (forceNewSearch == false && compressionFilesList.Count > 0)
                return true;

            compressionFilesList.Clear();

            string[] directoryGameFiles = Directory.GetFiles(Constants._gameFilesDirectory);
            foreach (string gameFile in directoryGameFiles)
            {
                compressionFilesList.Add(gameFile);
            }

            return compressionFilesList.Count > 0;
        }
        private bool GetCompressionSettings()
        {
            if (Directory.Exists(Constants._gameFilesDirectory))
            {
                if (ListCompressionFiles() == false)
                    return false;

                bool gameFilesCompressed = true;
                foreach (string compressionFile in compressionFilesList)
                {
                    FileAttributes fileAttributes = File.GetAttributes(compressionFile);
                    if ((fileAttributes & FileAttributes.Compressed) != FileAttributes.Compressed)
                    {
                        gameFilesCompressed = false;
                        break;
                    }
                }

                return gameFilesCompressed;
            }
            else
            {
                return false;
            }
        }
        private void SetCompressionSettings(bool enable)
        {
            if (Directory.Exists(Constants._gameFilesDirectory))
            {
                if (GetCompressionSettings() != enable)
                {
                    if (ListCompressionFiles() == false)
                        return;

                    foreach (string compressionFile in compressionFilesList)
                    {
                        if (GetCompressionProgressRunning() == false)
                            throw new InvalidOperationException("SetCompressionSettings can't be running while GetCompressionProgressRunning() == false");

                        string fileSafeName = compressionFile.Replace("\\", @"\\");

                        string objPath = $"CIM_DataFile.Name='{fileSafeName}'";
                        using (ManagementObject file = new ManagementObject(objPath))
                        {
                            try
                            {
                                file.InvokeMethod(enable ? "Compress" : "Uncompress", null, null);
                            }
                            catch (ManagementException ex)
                            {
                                MessageBox.Show($"Failed to update \"{compressionFile}\" file compression settings!\n\n{ex}", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                }
            }
        }
        public bool GetCompressionProgressRunning()
        {
            return _compressionProgress.isRunning;
        }




        private E_OS GetOS()
        {
            string winePrefix = Environment.GetEnvironmentVariable("WINELOADER");

            if (string.IsNullOrEmpty(winePrefix))
            {
                return E_OS.WINDOWS;
            }
            else
            {
                return E_OS.LINUX;
            }
        }



        private E_DiskType GetDiskType()
        {
            try
            {
                string driveRoot = Path.GetPathRoot(Constants._currentDirectory);
                string driveLetter = driveRoot.Substring(0, driveRoot.IndexOf(":\\"));
                List<string> problematicManufacturers = new List<string>
                {
                "ADATA",
                "APACER",
                "KINGSTON_S"
                };


                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
                ManagementObjectCollection collection = searcher.Get();
                foreach (ManagementObject obj in collection)
                {
                    string PNPDeviceID = obj["PNPDeviceID"] != null ? obj["PNPDeviceID"].ToString().ToUpper() : "NULL";
                    bool problematicManufacturer = false;

                    foreach (string manufacturer in problematicManufacturers)
                    {
                        if (PNPDeviceID.Contains(manufacturer))
                        {
                            problematicManufacturer = true;
                        }
                    }

                    if (PNPDeviceID.Contains("SSD") || PNPDeviceID.Contains("NVME") || PNPDeviceID.Contains("SATA") || problematicManufacturer == true)
                    {
                        foreach (ManagementObject partition in obj.GetRelated("Win32_DiskPartition"))
                        {
                            foreach (ManagementObject drive in partition.GetRelated("Win32_LogicalDisk"))
                            {
                                if ((UInt32)drive["DriveType"] == (UInt32)DriveType.Fixed)
                                {
                                    if (drive["Name"].ToString().StartsWith(driveLetter))
                                    {
                                        return E_DiskType.SSD;
                                    }
                                }
                            }
                        }
                    }
                }

                return E_DiskType.HDD;
            }
            catch
            {
                return E_DiskType.HDD;
            }
        }



        private void InitializeSettings()
        {
            if (CheckSettingsFile() == false)
            {
                MessageBox.Show($"WolverineLauncher failed to access \"{Constants._settingsPath}\"!\n\nMake sure that file exist in first place and there's no \"Read Only\" property assigned to it.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }

            #region Settings Page 01 (Main)
            // SETTINGS - RENDERAPI [01]
            if (WinReg.GetSubKeyExist())
            {
                switch (GetRenderingAPI())
                {
                    case E_RenderingAPI.D3D11:
                        comboBox_RenderAPI.SelectedIndex = 0;
                        break;

                    case E_RenderingAPI.VULKAN:
                        comboBox_RenderAPI.SelectedIndex = 1;
                        break;

                    case E_RenderingAPI.RESOREP:
                        comboBox_RenderAPI.Enabled = false;
                        comboBox_RenderAPI.Items.Add("DirectX 11 (Modded - Resorep)");
                        comboBox_RenderAPI.SelectedIndex = 2;
                        break;

                    case E_RenderingAPI.RESHADE:
                        comboBox_RenderAPI.Enabled = false;
                        comboBox_RenderAPI.Items.Add("DirectX 11 (Modded - ReShade)");
                        comboBox_RenderAPI.SelectedIndex = 2;
                        break;

                    case E_RenderingAPI.UNKNOWN:
                        MessageBox.Show($"WolverineLauncher failed to determine current rendering API!\n\n\"workspace\" folder might be missing?", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            else
            {
                SetRenderingAPI(E_RenderingAPI.D3D11);
                comboBox_RenderAPI.SelectedIndex = 0;
            }



            // SETTINGS - LANGUAGE [01]
            comboBox_Language.SelectedIndex = 0;
            //string language = WinReg.GetData_SZ("Language");
            //if (language == null)
            //{
            //    switch (Constants._cultureTwoLetterCode)
            //    {
            //        case "en":
            //            language = "us";
            //            break;
            //
            //        case "es":
            //            language = "es";
            //            break;
            //
            //        case "zh":
            //            language = "cs";
            //            break;
            //
            //        case "ar":
            //            language = "ar";
            //            break;
            //
            //        case "pt":
            //            language = "pt";
            //            break;
            //
            //        case "ru":
            //            language = "ru";
            //            break;
            //
            //        case "ja":
            //            language = "jp";
            //            break;
            //
            //        case "de":
            //            language = "de";
            //            break;
            //
            //        case "fr":
            //            language = "fr";
            //            break;
            //
            //        case "ko":
            //            language = "kr";
            //            break;
            //
            //        case "it":
            //            language = "it";
            //            break;
            //
            //        case "nl":
            //            language = "nl";
            //            break;
            //
            //        case "sv":
            //            language = "se";
            //            break;
            //
            //        case "pl":
            //            language = "pl";
            //            break;
            //
            //        case "no":
            //            language = "no";
            //            break;
            //
            //        case "da":
            //            language = "dk";
            //            break;
            //
            //        case "fi":
            //            language = "fi";
            //            break;
            //
            //        case "cs":
            //            language = "cz";
            //            break;
            //
            //        case "hu":
            //            language = "hu";
            //            break;
            //
            //        case "el":
            //            language = "el";
            //            break;
            //
            //        default:
            //            language = "us";
            //            break;
            //    }
            //    UpdateRegistry("Language", language, true); // For DiscordRPC, we need to have this key set in registry
            //}
            //UpdateStartupArguments("-language", $"-language {language}");
            //int languageIndex = Enum.IsDefined(typeof(E_LanguageIndex), language) ? (int)Enum.Parse(typeof(E_LanguageIndex), language) : 0;
            //comboBox_Language.SelectedIndex = languageIndex;



            // SETTINGS - ADMINISTRATOR MODE [01]
            string administratorMode = WinReg.GetData_SZ("Administrator Mode");
            switch (administratorMode)
            {
                case "Disabled":
                    comboBox_AdministratorMode.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_AdministratorMode.SelectedIndex = 1;
                    break;

                default:
                    comboBox_AdministratorMode.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - GAME MODE [01]
            string gameMode = WinReg.GetData_SZ("Game Mode");
            switch (gameMode)
            {
                case "Default":
                    comboBox_GameMode.SelectedIndex = 0;
                    break;

                case "Demo_Press":
                    comboBox_GameMode.SelectedIndex = 1;
                    break;

                default:
                    comboBox_GameMode.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - DEBUG MODE [01]
            string debugMode = WinReg.GetData_SZ("Debug Mode");
            switch (debugMode)
            {
                case "Minimum":
                    comboBox_DebugMode.SelectedIndex = 0;
                    break;

                case "Partial":
                    comboBox_DebugMode.SelectedIndex = 1;
                    break;

                case "Full":
                    comboBox_DebugMode.SelectedIndex = 2;
                    break;

                default:
                    comboBox_DebugMode.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - CONTROLLER VIBRATION [01]
            string controllerVibration = WinReg.GetData_SZ("Controller Vibration");
            switch (controllerVibration)
            {
                case "Disabled":
                    comboBox_ControllerVibration.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_ControllerVibration.SelectedIndex = 1;
                    break;

                default:
                    comboBox_ControllerVibration.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - CONTROLLER SPEAKER [01]
            string controllerSpeaker = WinReg.GetData_SZ("Controller Speaker");
            switch (controllerSpeaker)
            {
                case "Disabled":
                    comboBox_ControllerSpeaker.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_ControllerSpeaker.SelectedIndex = 1;
                    break;

                default:
                    comboBox_ControllerSpeaker.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - PERFORMANCE STATS [01]
            string performanceStats = WinReg.GetData_SZ("Performance Stats");
            switch (performanceStats)
            {
                case "Disabled":
                    comboBox_PerformanceStats.SelectedIndex = 0;
                    break;

                case "Minimalistic":
                    comboBox_PerformanceStats.SelectedIndex = 1;
                    break;

                case "Advanced":
                    comboBox_PerformanceStats.SelectedIndex = 2;
                    break;

                case "Debug":
                    comboBox_PerformanceStats.SelectedIndex = 3;
                    break;

                default:
                    comboBox_PerformanceStats.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - DEBUG CONTROLS [01]
            string debugControls = WinReg.GetData_SZ("Debug Controls");
            switch (debugControls)
            {
                case "Disabled":
                    comboBox_DebugControls.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_DebugControls.SelectedIndex = 1;
                    break;

                default:
                    comboBox_DebugControls.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - WINDOW MODE [01]
            string windowMode = WinReg.GetData_SZ("Window Mode");
            switch (windowMode)
            {
                case "Windowed":
                    comboBox_WindowMode.SelectedIndex = 0;
                    break;
            
                case "Borderless":
                    comboBox_WindowMode.SelectedIndex = 1;
                    break;

                case "Full Screen":
                    comboBox_WindowMode.SelectedIndex = 2;
                    break;

                default:
                    comboBox_WindowMode.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - WINDOW RESOLUTION [01]
            string windowResolution = WinReg.GetData_SZ("Window Resolution");
            switch (windowResolution)
            {
                case "Default":
                    comboBox_WindowResolution.SelectedIndex = 0;
                    break;

                case "3840x2160":
                    comboBox_WindowResolution.SelectedIndex = 1;
                    break;

                case "2560x1440":
                    comboBox_WindowResolution.SelectedIndex = 2;
                    break;

                case "1920x1080":
                    comboBox_WindowResolution.SelectedIndex = 3;
                    break;

                case "1600x900":
                    comboBox_WindowResolution.SelectedIndex = 4;
                    break;

                case "1280x720":
                    comboBox_WindowResolution.SelectedIndex = 5;
                    break;

                case "854x480":
                    comboBox_WindowResolution.SelectedIndex = 6;
                    break;

                case "426x240":
                    comboBox_WindowResolution.SelectedIndex = 7;
                    break;

                default:
                    comboBox_WindowResolution.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - ASPECT RATIO [01]
            string aspectRatio = WinReg.GetData_SZ("Aspect Ratio");
            switch (aspectRatio)
            {
                case "Default":
                    comboBox_AspectRatio.SelectedIndex = 0;
                    break;

                case "1.333":
                    comboBox_AspectRatio.SelectedIndex = 1;
                    break;

                case "1.777":
                    comboBox_AspectRatio.SelectedIndex = 2;
                    break;

                case "2.4":
                    comboBox_AspectRatio.SelectedIndex = 3;
                    break;

                default:
                    comboBox_AspectRatio.SelectedIndex = 2;
                    break;
            }



            // SETTINGS - UPSCALING [01]
            string upscaling = WinReg.GetData_SZ("Upscaling");
            switch (upscaling)
            {
                case "Disabled":
                    comboBox_Upscaling.SelectedIndex = 0;
                    break;

                case "Ultra Performance":
                    comboBox_Upscaling.SelectedIndex = 1;
                    break;

                case "Performance":
                    comboBox_Upscaling.SelectedIndex = 2;
                    break;

                case "Balanced":
                    comboBox_Upscaling.SelectedIndex = 3;
                    break;

                case "Quality":
                    comboBox_Upscaling.SelectedIndex = 4;
                    break;

                case "Ultra Quality":
                    comboBox_Upscaling.SelectedIndex = 5;
                    break;

                default:
                    comboBox_Upscaling.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - ANTI-ALIASING [01]
            string antiAliasing = WinReg.GetData_SZ("Anti-Aliasing");
            switch (antiAliasing)
            {
                case "Disabled":
                    comboBox_AntiAliasing.SelectedIndex = 0;
                    break;

                case "TAA":
                    comboBox_AntiAliasing.SelectedIndex = 1;
                    break;

                default:
                    comboBox_AntiAliasing.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - SAVEGAME DIRECTORY [01]
            button_SaveGameDirectory.Enabled = Directory.Exists(Constants._saveGameDirectory);
            #endregion








            #region Settings Page 02 (Graphics)
            // SETTINGS - FRAME RATE CAP [02]
            string frameRateCap = WinReg.GetData_SZ("Frame Rate Cap");
            switch (frameRateCap)
            {
                case "Default":
                    comboBox_FrameRateCap.SelectedIndex = 0;
                    break;

                case "Base":
                    comboBox_FrameRateCap.SelectedIndex = 1;
                    break;

                case "Extended":
                    comboBox_FrameRateCap.SelectedIndex = 2;
                    break;

                default:
                    comboBox_FrameRateCap.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - GEOMETRY QUALITY [02]
            string geometryQuality = WinReg.GetData_SZ("Geometry Quality");
            switch (geometryQuality)
            {
                case "Default":
                    comboBox_GeometryQuality.SelectedIndex = 0;
                    break;

                case "LOD0":
                    comboBox_GeometryQuality.SelectedIndex = 8;
                    break;

                case "LOD1":
                    comboBox_GeometryQuality.SelectedIndex = 7;
                    break;

                case "LOD2":
                    comboBox_GeometryQuality.SelectedIndex = 6;
                    break;

                case "LOD3":
                    comboBox_GeometryQuality.SelectedIndex = 5;
                    break;

                case "LOD4":
                    comboBox_GeometryQuality.SelectedIndex = 4;
                    break;

                case "LOD5":
                    comboBox_GeometryQuality.SelectedIndex = 3;
                    break;

                case "LOD6":
                    comboBox_GeometryQuality.SelectedIndex = 2;
                    break;

                case "LOD7":
                    comboBox_GeometryQuality.SelectedIndex = 1;
                    break;

                default:
                    comboBox_GeometryQuality.SelectedIndex = 0;
                    break;

            }



            // SETTINGS - IK FOOT [02]
            string ikFoot = WinReg.GetData_SZ("IKFoot");
            switch (ikFoot)
            {
                case "Disabled":
                    comboBox_IKFoot.SelectedIndex = 0;
                    break;

                case "Player Only":
                    comboBox_IKFoot.SelectedIndex = 1;
                    break;

                case "All":
                    comboBox_IKFoot.SelectedIndex = 2;
                    break;

                default:
                    comboBox_IKFoot.SelectedIndex = 2;
                    break;
            }



            // SETTINGS - SHADOW QUALITY [02]
            string shadowQuality = WinReg.GetData_SZ("Shadow Quality");
            switch (shadowQuality)
            {
                case "Low":
                    comboBox_ShadowQuality.SelectedIndex = 0;
                    break;

                case "Default":
                    comboBox_ShadowQuality.SelectedIndex = 1;
                    break;

                default:
                    comboBox_ShadowQuality.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - AMBIENT OCCLUSION [02]
            string ambientOcclusion = WinReg.GetData_SZ("Ambient Occlusion");
            switch (ambientOcclusion)
            {
                case "Low":
                    comboBox_AmbientOcclusion.SelectedIndex = 0;
                    break;

                case "Medium":
                    comboBox_AmbientOcclusion.SelectedIndex = 1;
                    break;

                case "High":
                    comboBox_AmbientOcclusion.SelectedIndex = 2;
                    break;

                default:
                    comboBox_AmbientOcclusion.SelectedIndex = 2;
                    break;
            }



            // SETTINGS - SCREEN SPACE REFLECTIONS (SSR) [02]
            string screenSpaceReflections = WinReg.GetData_SZ("SSR");
            switch (screenSpaceReflections)
            {
                case "Disabled":
                    comboBox_ScreenSpaceReflections.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_ScreenSpaceReflections.SelectedIndex = 1;
                    break;

                default:
                    comboBox_ScreenSpaceReflections.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - CLOUDS QUALITY [02]
            string cloudsQuality = WinReg.GetData_SZ("Clouds Quality");
            switch (cloudsQuality)
            {
                case "Minimum":
                    comboBox_CloudsQuality.SelectedIndex = 0;
                    break;

                case "Low":
                    comboBox_CloudsQuality.SelectedIndex = 1;
                    break;

                case "Medium":
                    comboBox_CloudsQuality.SelectedIndex = 2;
                    break;

                case "High":
                    comboBox_CloudsQuality.SelectedIndex = 3;
                    break;

                case "Ultra":
                    comboBox_CloudsQuality.SelectedIndex = 4;
                    break;

                case "Maximum":
                    comboBox_CloudsQuality.SelectedIndex = 5;
                    break;

                default:
                    comboBox_CloudsQuality.SelectedIndex = 4;
                    break;
            }



            // SETTINGS - FOG [02]
            string fog = WinReg.GetData_SZ("Fog");
            switch (fog)
            {
                case "Disabled":
                    comboBox_Fog.SelectedIndex = 0;
                    break;

                case "Distant Fog Only":
                    comboBox_Fog.SelectedIndex = 1;
                    break;

                case "Close Fog Only":
                    comboBox_Fog.SelectedIndex = 2;
                    break;

                case "Close Fog + Distant Fog":
                    comboBox_Fog.SelectedIndex = 3;
                    break;

                default:
                    comboBox_Fog.SelectedIndex = 3;
                    break;
            }



            // SETTINGS - CALCULATE SPLINE MESH DEVICE | CALCULATE SPLINE MESH [02]
            string calculateSplineMesh = WinReg.GetData_SZ("Calculate Spline Mesh Device");
            switch (calculateSplineMesh)
            {
                case "GPU":
                    comboBox_CalculateSplineMesh.SelectedIndex = 0;
                    break;

                case "CPU":
                    comboBox_CalculateSplineMesh.SelectedIndex = 1;
                    break;

                default:
                    comboBox_CalculateSplineMesh.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - PAUSE WORLD MAP [02]
            string pauseWorldMap = WinReg.GetData_SZ("Pause World Map");
            switch (pauseWorldMap)
            {
                case "Default":
                    comboBox_PauseWorldMap.SelectedIndex = 0;
                    break;

                case "Realistic":
                    comboBox_PauseWorldMap.SelectedIndex = 1;
                    break;

                default:
                    comboBox_PauseWorldMap.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - COLOR CORRECTION [02]
            string colorCorrection = WinReg.GetData_SZ("Color Correction");
            switch (colorCorrection)
            {
                case "Disabled":
                    comboBox_ColorCorrection.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_ColorCorrection.SelectedIndex = 1;
                    break;

                default:
                    comboBox_ColorCorrection.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - CHROMATIC ABERRATION [02]
            string chromaticAberration = WinReg.GetData_SZ("Chromatic Aberration");
            switch (chromaticAberration)
            {
                case "Disabled":
                    comboBox_ChromaticAberration.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_ChromaticAberration.SelectedIndex = 1;
                    break;

                default:
                    comboBox_ChromaticAberration.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - FILM GRAIN [02]
            string filmGrain = WinReg.GetData_SZ("Film Grain");
            switch (filmGrain)
            {
                case "Disabled":
                    comboBox_FilmGrain.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_FilmGrain.SelectedIndex = 1;
                    break;

                default:
                    comboBox_FilmGrain.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - MOTION BLUR [02]
            string motionBlur = WinReg.GetData_SZ("Motion Blur");
            switch (motionBlur)
            {
                case "Disabled":
                    comboBox_MotionBlur.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_MotionBlur.SelectedIndex = 1;
                    break;

                default:
                    comboBox_MotionBlur.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - DEPTH OF FIELD [02]
            string depthOfField = WinReg.GetData_SZ("Depth Of Field");
            switch (depthOfField)
            {
                case "Disabled":
                    comboBox_DepthOfField.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_DepthOfField.SelectedIndex = 1;
                    break;

                default:
                    comboBox_DepthOfField.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - BLOOM [02]
            string bloom = WinReg.GetData_SZ("Bloom");
            switch (bloom)
            {
                case "Disabled":
                    comboBox_Bloom.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_Bloom.SelectedIndex = 1;
                    break;

                default:
                    comboBox_Bloom.SelectedIndex = 1;
                    break;
            }
            #endregion








            #region Settings Page 03 (Other)
            // SETTINGS - ANSEL [03]
            string ansel = WinReg.GetData_SZ("Ansel");
            switch (ansel)
            {
                case "Disabled":
                    comboBox_Ansel.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_Ansel.SelectedIndex = 1;
                    break;

                default:
                    comboBox_Ansel.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - STARTUP INTRO [03]
            string startupIntro = WinReg.GetData_SZ("Startup Intro");
            switch (startupIntro)
            {
                case "Disabled":
                    comboBox_StartupIntro.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_StartupIntro.SelectedIndex = 1;
                    break;

                default:
                    comboBox_StartupIntro.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - MAIN MENU [03]
            string mainMenu = WinReg.GetData_SZ("Main Menu");
            switch (mainMenu)
            {
                case "Disabled":
                    comboBox_MainMenu.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_MainMenu.SelectedIndex = 1;
                    break;

                default:
                    comboBox_MainMenu.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - GODMODE [03]
            string godmode = WinReg.GetData_SZ("Godmode");
            switch (godmode)
            {
                case "Disabled":
                    comboBox_Godmode.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_Godmode.SelectedIndex = 1;
                    break;

                default:
                    comboBox_Godmode.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - OPERATOR MODE [03]
            string operatorMode = WinReg.GetData_SZ("Operator Mode");
            switch (operatorMode)
            {
                case "Disabled":
                    comboBox_OperatorMode.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_OperatorMode.SelectedIndex = 1;
                    break;

                default:
                    comboBox_OperatorMode.SelectedIndex = 0;
                    break;
            }



            // SETTINGS - CAPTURE FORMAT [03]
            string captureFormat = WinReg.GetData_SZ("Capture Format");
            switch (captureFormat)
            {
                case "bmp":
                    comboBox_CaptureFormat.SelectedIndex = 0;
                    break;

                case "dds":
                    comboBox_CaptureFormat.SelectedIndex = 1;
                    break;

                case "jpg":
                    comboBox_CaptureFormat.SelectedIndex = 2;
                    break;

                case "png":
                    comboBox_CaptureFormat.SelectedIndex = 3;
                    break;

                case "hdr":
                    comboBox_CaptureFormat.SelectedIndex = 4;
                    break;

                default:
                    comboBox_CaptureFormat.SelectedIndex = 3;
                    break;
            }



            // SETTINGS - CAPTURE FORMAT [03]
            string captureResolution = WinReg.GetData_SZ("Capture Resolution");
            switch (captureResolution)
            {
                case "Default":
                    comboBox_CaptureResolution.SelectedIndex = 0;
                    break;

                case "256 Square":
                    comboBox_CaptureResolution.SelectedIndex = 1;
                    break;

                case "512 Square":
                    comboBox_CaptureResolution.SelectedIndex = 2;
                    break;

                case "1080p":
                    comboBox_CaptureResolution.SelectedIndex = 3;
                    break;

                case "1440p":
                    comboBox_CaptureResolution.SelectedIndex = 4;
                    break;

                case "4K":
                    comboBox_CaptureResolution.SelectedIndex = 5;
                    break;

                case "8K":
                    comboBox_CaptureResolution.SelectedIndex = 6;
                    break;

                case "8192 Square":
                    comboBox_CaptureResolution.SelectedIndex = 7;
                    break;

                case "10K":
                    comboBox_CaptureResolution.SelectedIndex = 8;
                    break;

                default:
                    comboBox_CaptureResolution.SelectedIndex = 5;
                    break;
            }



            //SETTINGS - WORLD STREAMING [03]
            string worldStreaming = WinReg.GetData_SZ("World Streaming");
            switch (worldStreaming)
            {
                case "Default":
                    comboBox_WorldStreaming.SelectedIndex = 0;
                    break;

                case "High Budget":
                    comboBox_WorldStreaming.SelectedIndex = 1;
                    break;

                default:
                    comboBox_WorldStreaming.SelectedIndex = 0;
                    break;

            }



            //SETTINGS - DELAYED STREAMING [03]
            string delayedStreaming = WinReg.GetData_SZ("Delayed Streaming");
            switch (delayedStreaming)
            {
                case "Disabled":
                    comboBox_DelayedStreaming.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_DelayedStreaming.SelectedIndex = 1;
                    break;

                default:
                    comboBox_DelayedStreaming.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - COMPOSITE MATERIALS [03]
            string compositeMaterials = WinReg.GetData_SZ("Composite Materials");
            switch (compositeMaterials)
            {
                case "Disabled":
                    comboBox_CompositeMaterials.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_CompositeMaterials.SelectedIndex = 1;
                    break;

                default:
                    comboBox_CompositeMaterials.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - DECALS [03]
            string decals = WinReg.GetData_SZ("Decals");
            switch (decals)
            {
                case "Disabled":
                    comboBox_Decals.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_Decals.SelectedIndex = 1;
                    break;

                default:
                    comboBox_Decals.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - WEATHER [03]
            string weather = WinReg.GetData_SZ("Weather");
            switch (weather)
            {
                case "Disabled":
                    comboBox_Weather.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_Weather.SelectedIndex = 1;
                    break;

                default:
                    comboBox_Weather.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - VFX [03]
            string vfx = WinReg.GetData_SZ("VFX");
            switch (vfx)
            {
                case "Disabled":
                    comboBox_VFX.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_VFX.SelectedIndex = 1;
                    break;

                default:
                    comboBox_VFX.SelectedIndex = 1;
                    break;
            }



            // SETTINGS - OUTLINES [03]
            string outlines = WinReg.GetData_SZ("Outlines");
            switch (outlines)
            {
                case "Disabled":
                    comboBox_Outlines.SelectedIndex = 0;
                    break;

                case "Enabled":
                    comboBox_Outlines.SelectedIndex = 1;
                    break;

                default:
                    comboBox_Outlines.SelectedIndex = 1;
                    break;
            }
            #endregion








            #region Settings Page 04 (Experiments)
            // EXPERIMENTS - DISCORD RPC [04]
            switch (GetExperimentState(_experiment_DiscordRPC))
            {
                case E_ExperimentState.DISABLED:
                    comboBox_Experiment_DiscordRPC.SelectedIndex = 0;
                    break;

                case E_ExperimentState.ENABLED:
                    comboBox_Experiment_DiscordRPC.SelectedIndex = 1;
                    break;

                case E_ExperimentState.UNKNOWN:
                    MessageBox.Show($"WolverineLauncher failed to determine \"{label_Experiment_DiscordRPC.Text}\" experiment state! \"..\\workspace\\plugins\" folder might be missing?\n\n\"{_experiment_DiscordRPC.path}\"\n\n\"{_experiment_DiscordRPC.disabledPath}\"", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }


            
            // EXPERIMENTS - NO CRASH WINDOW [04]
            switch (GetExperimentState(_experiment_NoCrashWindow))
            {
                case E_ExperimentState.DISABLED:
                    comboBox_Experiment_NoCrashWindow.SelectedIndex = 0;
                    break;

                case E_ExperimentState.ENABLED:
                    comboBox_Experiment_NoCrashWindow.SelectedIndex = 1;
                    break;

                case E_ExperimentState.UNKNOWN:
                    MessageBox.Show($"WolverineLauncher failed to determine \"{label_Experiment_NoCrashWindow.Text}\" experiment state! \"..\\workspace\\plugins\" folder might be missing?\n\n\"{_experiment_NoCrashWindow.path}\"\n\n\"{_experiment_NoCrashWindow.disabledPath}\"", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
            }



            // EXPERIMENTS - COMPRESS GAME FILES [04]
            if (GetOS() == E_OS.WINDOWS)
            {
                switch (GetCompressionSettings())
                {
                    case false:
                        comboBox_Experiment_CompressGameFiles.SelectedIndex = 0;
                        break;

                    case true:
                        comboBox_Experiment_CompressGameFiles.SelectedIndex = 1;
                        break;
                }
            }
            else
            {
                label_Experiment_CompressGameFiles.Enabled = false;
                comboBox_Experiment_CompressGameFiles.Enabled = false;
            }
            #endregion








            // SETTINGS - CUSTOM GAME STARTUP ARGUMENTS
            string customGameStartupArguments = WinReg.GetData_SZ("Custom Game Startup Arguments");
            if (string.IsNullOrEmpty(customGameStartupArguments) == false)
            {
                textBox_CustomGameStartupArguments.Text = customGameStartupArguments; // textBox_CustomGameStartupArguments_TextChanged updates _customGameStartupArguments variable
            }








            if (GetDiskType() == E_DiskType.HDD)
            {
                label_HardDriveWarning.Visible = true;
            }








            // Once variable is True, Windows Registry will start getting updated
            _settingsInitialized = true;
        }
        private void DarkThemeAdaptation()
        {
            if (WinReg.GetDarkThemeEnabled())
            {
                pictureBox_Outline.BackColor = SystemColors.Window;

                panel_WindowHeader.BackColor = SystemColors.Window;
                label_WindowTitle.ForeColor = SystemColors.WindowText;
                labelButton_Close.BackColor = SystemColors.Window;
                labelButton_Close.ForeColor = SystemColors.ControlText;

                label_CustomGameStartupArguments.BackColor = SystemColors.Window;
                label_CustomGameStartupArguments.ForeColor = SystemColors.ControlText;

                button_OK.BackColor = SystemColors.Window;
                button_OK.ForeColor = SystemColors.ControlText;
            }
        }
        public Settings()
        {
            InitializeComponent();
            InitializeSettings();

            DarkThemeAdaptation();
            Handle_LanguageLabelAnimation();
        }
        private void Settings_Load(object sender, EventArgs e)
        {
            if (Constants._timePlayed > 0)
            {
                label_TimePlayed.Visible = true;

                if (Constants._timePlayed < 7200)
                {
                    int minutesPlayed = Constants._timePlayed / 60;

                    if (minutesPlayed < 1)
                    {
                        label_TimePlayed.Text = $"Time Played: {Constants._timePlayed} seconds";
                    }
                    else if (minutesPlayed < 2)
                    {
                        label_TimePlayed.Text = $"Time Played: {minutesPlayed} minute";
                    }
                    else
                    {
                        label_TimePlayed.Text = $"Time Played: {minutesPlayed} minutes";
                    }
                }
                else
                {
                    double hoursPlayed = (double)Constants._timePlayed / 3600;
                    label_TimePlayed.Text = $"Time Played: {(Math.Floor(hoursPlayed * 10) / 10).ToString().Replace(',', '.')} hours";
                }
            }




            checkBox_Warning.Checked = Overrides._proMode;
        }



        private async void panel_WindowHeader_MouseDown(object sender, MouseEventArgs e)
        {
            panel_WindowHeader.Capture = false;

            await Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    Message mouse = Message.Create(Handle, 0xa1, new IntPtr(2), IntPtr.Zero);
                    WndProc(ref mouse);
                }));
            });
        }
        private void labelButton_Close_MouseEnter(object sender, EventArgs e)
        {
            labelButton_Close.BackColor = Color.FromArgb(232, 17, 35);
            labelButton_Close.ForeColor = Color.White;
        }
        private void labelButton_Close_MouseLeave(object sender, EventArgs e)
        {
            labelButton_Close.BackColor = Color.Transparent;
            labelButton_Close.ForeColor = Color.Black;
        }
        private void labelButton_Close_MouseClick(object sender, MouseEventArgs e) => this.Hide();



        private void comboBox_RenderAPI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_settingsInitialized)
            {
                switch (comboBox_RenderAPI.SelectedIndex)
                {
                    case 0:
                        SetRenderingAPI(E_RenderingAPI.D3D11);
                        break;

                    case 1:
                        SetRenderingAPI(E_RenderingAPI.VULKAN);
                        break;
                }
            }
        }
        private void comboBox_Language_SelectedIndexChanged(object sender, EventArgs e)
        {
            string newLanguage = null;
            switch (comboBox_Language.SelectedIndex)
            {
                case 0:
                    newLanguage = "us";
                    break;

                case 1:
                    newLanguage = "es";
                    break;

                case 2:
                    newLanguage = "la";
                    break;

                case 3:
                    newLanguage = "cs";
                    break;

                case 4:
                    newLanguage = "ct";
                    break;

                case 5:
                    newLanguage = "ar";
                    break;

                case 6:
                    newLanguage = "pt";
                    break;

                case 7:
                    newLanguage = "br";
                    break;

                case 8:
                    newLanguage = "ru";
                    break;

                case 9:
                    newLanguage = "jp";
                    break;

                case 10:
                    newLanguage = "de";
                    break;

                case 11:
                    newLanguage = "fr";
                    break;

                case 12:
                    newLanguage = "fc";
                    break;

                case 13:
                    newLanguage = "kr";
                    break;

                case 14:
                    newLanguage = "it";
                    break;

                case 15:
                    newLanguage = "nl";
                    break;

                case 16:
                    newLanguage = "se";
                    break;

                case 17:
                    newLanguage = "pl";
                    break;

                case 18:
                    newLanguage = "no";
                    break;

                case 19:
                    newLanguage = "dk";
                    break;

                case 20:
                    newLanguage = "fi";
                    break;

                case 21:
                    newLanguage = "cz";
                    break;

                case 22:
                    newLanguage = "hu";
                    break;

                case 23:
                    newLanguage = "el";
                    break;

                default:
                    newLanguage = "us";
                    break;
            }

            UpdateStartupArguments("-language", $"-language {newLanguage}");
            UpdateRegistry("Language", newLanguage);
        }
        private void comboBox_AdministratorMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_AdministratorMode.SelectedIndex)
            {
                case 0:
                    _gameAdministratorMode = false;
                    UpdateRegistry("Administrator Mode", "Disabled");
                    break;

                case 1:
                    _gameAdministratorMode = true;
                    UpdateRegistry("Administrator Mode", "Enabled");
                    break;
            }
        }
        private void comboBox_GameMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_GameMode.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-demo_press", string.Empty);
                    UpdateRegistry("Game Mode", "Default");
                    break;


                case 1:
                    UpdatePreciseStartupArguments("-demo_press", "-demo_press");
                    UpdateRegistry("Game Mode", "Demo_Press");
                    break;
            }
        }
        private void comboBox_DebugMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_DebugMode.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-nodbgmsg", "-nodbgmsg");
                    UpdatePreciseStartupArguments("-noconsole", "-noconsole");
                    UpdatePreciseStartupArguments("-no_debug_spam", "-no_debug_spam");
                    UpdatePreciseStartupArguments("-no_tty", "-no_tty");
                    UpdatePreciseStartupArguments("-noprotomsg", "-noprotomsg");
                    UpdatePreciseStartupArguments("-no_error_actors", "-no_error_actors");
                    UpdatePreciseStartupArguments("-turn_off_all_script_msgs", "-turn_off_all_script_msgs");
                    UpdatePreciseStartupArguments("-perftest", "-perftest");
                    UpdatePreciseStartupArguments("-shipping", "-shipping");
                    UpdatePreciseStartupArguments("-herocharacterui", string.Empty);
                    UpdateSettings("dbgmenu, Enable TTY", "dbgmenu, Enable TTY, 0");
                    UpdateSettings("dbgmenu, Turn Off All Debug Draw", "dbgmenu, Turn Off All Debug Draw, 1");
                    UpdateRegistry("Debug Mode", "Minimum");

                    label_PerformanceStatsWarning.Visible = true;
                    label_DebugModeWarning.Visible = true;
                    comboBox_PerformanceStats.Enabled = false;

                    break;

                case 1:
                    UpdatePreciseStartupArguments("-nodbgmsg", "-nodbgmsg");
                    UpdatePreciseStartupArguments("-noconsole", "-noconsole");
                    UpdatePreciseStartupArguments("-no_debug_spam", "-no_debug_spam");
                    UpdatePreciseStartupArguments("-no_tty", "-no_tty");
                    UpdatePreciseStartupArguments("-noprotomsg", "-noprotomsg");
                    UpdatePreciseStartupArguments("-no_error_actors", "-no_error_actors");
                    UpdatePreciseStartupArguments("-turn_off_all_script_msgs", "-turn_off_all_script_msgs");
                    UpdatePreciseStartupArguments("-perftest", "-perftest");
                    UpdatePreciseStartupArguments("-shipping", string.Empty);
                    UpdatePreciseStartupArguments("-herocharacterui", string.Empty);
                    UpdateSettings("dbgmenu, Enable TTY", "dbgmenu, Enable TTY, 0");
                    UpdateSettings("dbgmenu, Turn Off All Debug Draw", "dbgmenu, Turn Off All Debug Draw, 0");
                    UpdateRegistry("Debug Mode", "Partial");

                    label_PerformanceStatsWarning.Visible = false;
                    label_DebugModeWarning.Visible = false;
                    comboBox_PerformanceStats.Enabled = true;

                    break;

                case 2:
                    UpdatePreciseStartupArguments("-nodbgmsg", string.Empty);
                    UpdatePreciseStartupArguments("-noconsole", string.Empty);
                    UpdatePreciseStartupArguments("-no_debug_spam", string.Empty);
                    UpdatePreciseStartupArguments("-no_tty", string.Empty);
                    UpdatePreciseStartupArguments("-noprotomsg", string.Empty);
                    UpdatePreciseStartupArguments("-no_error_actors", string.Empty);
                    UpdatePreciseStartupArguments("-turn_off_all_script_msgs", string.Empty);
                    UpdatePreciseStartupArguments("-perftest", string.Empty);
                    UpdatePreciseStartupArguments("-shipping", string.Empty);
                    UpdatePreciseStartupArguments("-herocharacterui", "-herocharacterui");
                    UpdateSettings("dbgmenu, Enable TTY", "dbgmenu, Enable TTY, 1");
                    UpdateSettings("dbgmenu, Turn Off All Debug Draw", "dbgmenu, Turn Off All Debug Draw, 0");
                    UpdateRegistry("Debug Mode", "Full");

                    label_PerformanceStatsWarning.Visible = false;
                    label_DebugModeWarning.Visible = false;
                    comboBox_PerformanceStats.Enabled = true;

                    break;
            }
        }
        private void comboBox_ControllerVibration_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_ControllerVibration.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-norumble", "-norumble");
                    UpdatePreciseStartupArguments("-rumble", string.Empty);
                    UpdateRegistry("Controller Vibration", "Disabled");
                    break;

                case 1:
                    UpdatePreciseStartupArguments("-norumble", string.Empty);
                    UpdatePreciseStartupArguments("-rumble", "-rumble");
                    UpdateRegistry("Controller Vibration", "Enabled");
                    break;
            }
        }
        private void comboBox_ControllerSpeaker_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_ControllerSpeaker.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-nocontrollerspeaker", "-nocontrollerspeaker");
                    UpdatePreciseStartupArguments("-controllerspeaker", string.Empty);
                    UpdateRegistry("Controller Speaker", "Disabled");

                    label_ControllerSpeakerWarning.Visible = true;
                    break;

                case 1:
                    UpdatePreciseStartupArguments("-nocontrollerspeaker", string.Empty);
                    UpdatePreciseStartupArguments("-controllerspeaker", "-controllerspeaker");
                    UpdateRegistry("Controller Speaker", "Enabled");

                    label_ControllerSpeakerWarning.Visible = false;
                    break;
            }
        }
        private void comboBox_PerformanceStats_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_PerformanceStats.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-show_fps", string.Empty);
                    UpdatePreciseStartupArguments("-time_graph", string.Empty);
                    UpdatePreciseStartupArguments("-time_histogram", string.Empty);
                    UpdatePreciseStartupArguments("-show_localtime", string.Empty);
                    UpdatePreciseStartupArguments("-memstats", string.Empty);
                    UpdatePreciseStartupArguments("-load_time_display_always", string.Empty);
                    UpdateRegistry("Performance Stats", "Disabled");
                    break;

                case 1:
                    UpdatePreciseStartupArguments("-show_fps", "-show_fps");
                    UpdatePreciseStartupArguments("-time_graph", string.Empty);
                    UpdatePreciseStartupArguments("-time_histogram", string.Empty);
                    UpdatePreciseStartupArguments("-show_localtime", string.Empty);
                    UpdatePreciseStartupArguments("-memstats", string.Empty);
                    UpdatePreciseStartupArguments("-load_time_display_always", string.Empty);
                    UpdateRegistry("Performance Stats", "Minimalistic");
                    break;

                case 2:
                    UpdatePreciseStartupArguments("-show_fps", "-show_fps");
                    UpdatePreciseStartupArguments("-time_graph", "-time_graph");
                    UpdatePreciseStartupArguments("-time_histogram", "-time_histogram");
                    UpdatePreciseStartupArguments("-show_localtime", string.Empty);
                    UpdatePreciseStartupArguments("-memstats", string.Empty);
                    UpdatePreciseStartupArguments("-load_time_display_always", string.Empty);
                    UpdateRegistry("Performance Stats", "Advanced");
                    break;

                case 3:
                    UpdatePreciseStartupArguments("-show_fps", "-show_fps");
                    UpdatePreciseStartupArguments("-time_graph", "-time_graph");
                    UpdatePreciseStartupArguments("-time_histogram", "-time_histogram");
                    UpdatePreciseStartupArguments("-show_localtime", "-show_localtime");
                    UpdatePreciseStartupArguments("-memstats", "-memstats");
                    UpdatePreciseStartupArguments("-load_time_display_always", "-load_time_display_always");
                    UpdateRegistry("Performance Stats", "Debug");
                    break;
            }
        }
        private void comboBox_DebugControls_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_DebugControls.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-nopadremoved", string.Empty);
                    UpdateRegistry("Debug Controls", "Disabled");
                    break;

                case 1:
                    UpdatePreciseStartupArguments("-nopadremoved", "-nopadremoved");
                    UpdateRegistry("Debug Controls", "Enabled");
                    break;
            }
        }
        private void comboBox_WindowMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_WindowMode.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-borderlesswindow", string.Empty);
                    UpdatePreciseStartupArguments("-customfullscreen", string.Empty);
                    UpdateRegistry("Window Mode", "Windowed");

                    comboBox_WindowResolution.Enabled = true;

                    break;

                case 1:
                    UpdatePreciseStartupArguments("-borderlesswindow", "-borderlesswindow");
                    UpdatePreciseStartupArguments("-customfullscreen", string.Empty);
                    UpdateRegistry("Window Mode", "Borderless");

                    comboBox_WindowResolution.Enabled = false;

                    break;

                case 2:
                    UpdatePreciseStartupArguments("-borderlesswindow", string.Empty);
                    UpdatePreciseStartupArguments("-customfullscreen", "-customfullscreen");
                    UpdateRegistry("Window Mode", "Full Screen");

                    comboBox_WindowResolution.Enabled = false;

                    break;
            }
        }
        private void comboBox_WindowResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_WindowResolution.SelectedIndex)
            {
                case 0:
                    UpdateStartupArguments("-res ", string.Empty);
                    UpdateRegistry("Window Resolution", "Default");
                    break;

                case 1:
                    UpdateStartupArguments("-res ", "-res 4k");
                    UpdateRegistry("Window Resolution", "3840x2160");
                    break;

                case 2:
                    UpdateStartupArguments("-res ", "-res 1440");
                    UpdateRegistry("Window Resolution", "2560x1440");
                    break;

                case 3:
                    UpdateStartupArguments("-res ", "-res 1080");
                    UpdateRegistry("Window Resolution", "1920x1080");
                    break;

                case 4:
                    UpdateStartupArguments("-res ", "-res 900");
                    UpdateRegistry("Window Resolution", "1600x900");
                    break;

                case 5:
                    UpdateStartupArguments("-res ", "-res 720");
                    UpdateRegistry("Window Resolution", "1280x720");
                    break;

                case 6:
                    UpdateStartupArguments("-res ", "-res 480");
                    UpdateRegistry("Window Resolution", "854x480");
                    break;

                case 7:
                    UpdateStartupArguments("-res ", "-res 240");
                    UpdateRegistry("Window Resolution", "426x240");
                    break;
            }
        }
        private void comboBox_AspectRatio_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_AspectRatio.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Aspect Ratio Override", "dbgmenu, Aspect Ratio Override, none");
                    UpdateRegistry("Aspect Ratio", "Default");

                    label_AspectRatioWarning.Visible = true;
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Aspect Ratio Override", "dbgmenu, Aspect Ratio Override, 1.333");  // 4:3 = 1.333
                    UpdateRegistry("Aspect Ratio", "1.333");

                    label_AspectRatioWarning.Visible = true;
                    break;

                case 2:
                    UpdateSettings("dbgmenu, Aspect Ratio Override", "dbgmenu, Aspect Ratio Override, 1.777"); // 16:9 = 1.777
                    UpdateRegistry("Aspect Ratio", "1.777");

                    label_AspectRatioWarning.Visible = false;
                    break;

                case 3:
                    UpdateSettings("dbgmenu, Aspect Ratio Override", "dbgmenu, Aspect Ratio Override, 2.4");  // 21:9 = 2.333
                    UpdateRegistry("Aspect Ratio", "2.4");

                    label_AspectRatioWarning.Visible = true;
                    break;
            }
        }
        private void comboBox_Upscaling_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Upscaling.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Temporal AA Upscale Scale", "dbgmenu, Temporal AA Upscale Scale, 1.0000");
                    UpdateSettings("dbgmenu, Apply Temporal AA Upscale Scale", "dbgmenu, Apply Temporal AA Upscale Scale, true");
                    UpdateRegistry("Upscaling", "Disabled");
                    label_UpscalingToolTip.Visible = false;
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Temporal AA Upscale Scale", "dbgmenu, Temporal AA Upscale Scale, 0.3333");
                    UpdateSettings("dbgmenu, Apply Temporal AA Upscale Scale", "dbgmenu, Apply Temporal AA Upscale Scale, true");
                    UpdateRegistry("Upscaling", "Ultra Performance");
                    label_UpscalingToolTip.Visible = true;
                    break;

                case 2:
                    UpdateSettings("dbgmenu, Temporal AA Upscale Scale", "dbgmenu, Temporal AA Upscale Scale, 0.5000");
                    UpdateSettings("dbgmenu, Apply Temporal AA Upscale Scale", "dbgmenu, Apply Temporal AA Upscale Scale, true");
                    UpdateRegistry("Upscaling", "Performance");
                    label_UpscalingToolTip.Visible = true;
                    break;

                case 3:
                    UpdateSettings("dbgmenu, Temporal AA Upscale Scale", "dbgmenu, Temporal AA Upscale Scale, 0.5800");
                    UpdateSettings("dbgmenu, Apply Temporal AA Upscale Scale", "dbgmenu, Apply Temporal AA Upscale Scale, true");
                    UpdateRegistry("Upscaling", "Balanced");
                    label_UpscalingToolTip.Visible = true;
                    break;

                case 4:
                    UpdateSettings("dbgmenu, Temporal AA Upscale Scale", "dbgmenu, Temporal AA Upscale Scale, 0.6666");
                    UpdateSettings("dbgmenu, Apply Temporal AA Upscale Scale", "dbgmenu, Apply Temporal AA Upscale Scale, true");
                    UpdateRegistry("Upscaling", "Quality");
                    label_UpscalingToolTip.Visible = true;
                    break;

                case 5:
                    UpdateSettings("dbgmenu, Temporal AA Upscale Scale", "dbgmenu, Temporal AA Upscale Scale, 0.8000");
                    UpdateSettings("dbgmenu, Apply Temporal AA Upscale Scale", "dbgmenu, Apply Temporal AA Upscale Scale, true");
                    UpdateRegistry("Upscaling", "Ultra Quality");
                    label_UpscalingToolTip.Visible = true;
                    break;
            }
        }
        private void comboBox_AntiAliasing_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_AntiAliasing.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Anti-Aliasing Mode", "dbgmenu, Anti-Aliasing Mode, off");
                    UpdateRegistry("Anti-Aliasing", "Disabled");

                    label_UpscalingWarning.Visible = true;
                    comboBox_Upscaling.Enabled = false;

                    break;

                case 1:
                    UpdateSettings("dbgmenu, Anti-Aliasing Mode", "dbgmenu, Anti-Aliasing Mode, Temporal");
                    UpdateRegistry("Anti-Aliasing", "TAA");

                    label_UpscalingWarning.Visible = false;
                    comboBox_Upscaling.Enabled = true;

                    break;
            }
        }








        private void comboBox_FrameRateCap_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_FrameRateCap.SelectedIndex)
            {
                case 0:
                    UpdateStartupArguments("-fps_unlock", string.Empty);
                    UpdateRegistry("Frame Rate Cap", "Default");
                    label_FrameRateCapToolTip.Visible = false;
                    break;

                case 1:
                    UpdateStartupArguments("-fps_unlock", "-fps_unlock_base");
                    UpdateRegistry("Frame Rate Cap", "Base");
                    label_FrameRateCapToolTip.Visible = false;
                    break;

                case 2:
                    UpdateStartupArguments("-fps_unlock", "-fps_unlock_extended");
                    UpdateRegistry("Frame Rate Cap", "Extended");
                    label_FrameRateCapToolTip.Visible = true;
                    break;
            }
        }
        private void comboBox_GeometryQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            string newGeometryQuality = null;
            string newGeometryQuality_Internal = null;
            switch (comboBox_GeometryQuality.SelectedIndex)
            {
                case 0:
                    newGeometryQuality = "Default";
                    newGeometryQuality_Internal = "0";
                    break;

                case 1:
                    newGeometryQuality = "LOD7";
                    newGeometryQuality_Internal = "8";
                    break;

                case 2:
                    newGeometryQuality = "LOD6";
                    newGeometryQuality_Internal = "7";
                    break;

                case 3:
                    newGeometryQuality = "LOD5";
                    newGeometryQuality_Internal = "6";
                    break;

                case 4:
                    newGeometryQuality = "LOD4";
                    newGeometryQuality_Internal = "5";
                    break;

                case 5:
                    newGeometryQuality = "LOD3";
                    newGeometryQuality_Internal = "4";
                    break;

                case 6:
                    newGeometryQuality = "LOD2";
                    newGeometryQuality_Internal = "3";
                    break;

                case 7:
                    newGeometryQuality = "LOD1";
                    newGeometryQuality_Internal = "2";
                    break;

                case 8:
                    newGeometryQuality = "LOD0";
                    newGeometryQuality_Internal = "1";
                    break;
            }

            UpdateSettings("dbgmenu, Geom Lod Level", $"dbgmenu, Geom Lod Level, {newGeometryQuality_Internal}");
            UpdateRegistry("Geometry Quality", newGeometryQuality);
        }
        private void comboBox_IKFoot_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_IKFoot.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, IK Enabled", "dbgmenu, IK Enabled, 0");
                    UpdateSettings("dbgmenu, Enable IK on peds", "dbgmenu, Enable IK on peds, 0");
                    UpdateRegistry("IKFoot", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, IK Enabled", "dbgmenu, IK Enabled, 1");
                    UpdateSettings("dbgmenu, Enable IK on peds", "dbgmenu, Enable IK on peds, 0");
                    UpdateRegistry("IKFoot", "Player Only");
                    break;

                case 2:
                    UpdateSettings("dbgmenu, IK Enabled", "dbgmenu, IK Enabled, 1");
                    UpdateSettings("dbgmenu, Enable IK on peds", "dbgmenu, Enable IK on peds, 1");
                    UpdateRegistry("IKFoot", "All");
                    break;
            }
        }
        private void comboBox_ShadowQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_ShadowQuality.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Force Disable", "dbgmenu, Force Disable, 1");
                    UpdateRegistry("Shadow Quality", "Low");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Force Disable", "dbgmenu, Force Disable, 0");
                    UpdateRegistry("Shadow Quality", "Default");
                    break;
            }
        }
        private void comboBox_AmbientOcclusion_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_AmbientOcclusion.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Baked AO", "dbgmenu, Enable Baked AO, 1");
                    UpdateSettings("dbgmenu, Enable Screen Space AO", "dbgmenu, Enable Screen Space AO, 1");
                    UpdateRegistry("Ambient Occlusion", "Low");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Baked AO", "dbgmenu, Enable Baked AO, 0");
                    UpdateSettings("dbgmenu, Enable Screen Space AO", "dbgmenu, Enable Screen Space AO, 1");
                    UpdateRegistry("Ambient Occlusion", "Medium");
                    break;

                case 2:
                    UpdateSettings("dbgmenu, Enable Baked AO", "dbgmenu, Enable Baked AO, 0");
                    UpdateSettings("dbgmenu, Enable Screen Space AO", "dbgmenu, Enable Screen Space AO, 0");
                    UpdateRegistry("Ambient Occlusion", "High");
                    break;
            }
        }
        private void comboBox_ScreenSpaceReflections_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_ScreenSpaceReflections.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Screen Space Refl", "dbgmenu, Enable Screen Space Refl, 1");
                    UpdateRegistry("SSR", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Screen Space Refl", "dbgmenu, Enable Screen Space Refl, 0");
                    UpdateRegistry("SSR", "Enabled");
                    break;
            }
        }
        private void comboBox_CloudsQuality_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_CloudsQuality.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Ray Marching Steps", "dbgmenu, Ray Marching Steps, 64");
                    UpdateSettings("dbgmenu, Temporal Upsampling Mode", "dbgmenu, Temporal Upsampling Mode, 4x4");
                    UpdateRegistry("Clouds Quality", "Minimum");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Ray Marching Steps", "dbgmenu, Ray Marching Steps, 128");
                    UpdateSettings("dbgmenu, Temporal Upsampling Mode", "dbgmenu, Temporal Upsampling Mode, 4x4");
                    UpdateRegistry("Clouds Quality", "Low");
                    break;

                case 2:
                    UpdateSettings("dbgmenu, Ray Marching Steps", "dbgmenu, Ray Marching Steps, 256");
                    UpdateSettings("dbgmenu, Temporal Upsampling Mode", "dbgmenu, Temporal Upsampling Mode, 4x4");
                    UpdateRegistry("Clouds Quality", "Medium");
                    break;

                case 3:
                    UpdateSettings("dbgmenu, Ray Marching Steps", "dbgmenu, Ray Marching Steps, 512");
                    UpdateSettings("dbgmenu, Temporal Upsampling Mode", "dbgmenu, Temporal Upsampling Mode, 4x4");
                    UpdateRegistry("Clouds Quality", "High");
                    break;

                case 4:
                    UpdateSettings("dbgmenu, Ray Marching Steps", "dbgmenu, Ray Marching Steps, 1024");
                    UpdateSettings("dbgmenu, Temporal Upsampling Mode", "dbgmenu, Temporal Upsampling Mode, 4x4");
                    UpdateRegistry("Clouds Quality", "Ultra");
                    break;

                case 5:
                    UpdateSettings("dbgmenu, Ray Marching Steps", "dbgmenu, Ray Marching Steps, 2048");
                    UpdateSettings("dbgmenu, Temporal Upsampling Mode", "dbgmenu, Temporal Upsampling Mode, 4x4");
                    UpdateRegistry("Clouds Quality", "Maximum");
                    break;
            }
        }
        private void comboBox_Fog_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Fog.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Fog", "dbgmenu, Enable Fog, 1");
                    UpdateSettings("dbgmenu, Enable vFog", "dbgmenu, Enable vFog, 1");
                    UpdateRegistry("Fog", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Fog", "dbgmenu, Enable Fog, 1");
                    UpdateSettings("dbgmenu, Enable vFog", "dbgmenu, Enable vFog, 0");
                    UpdateRegistry("Fog", "Distant Fog Only");
                    break;

                case 2:
                    UpdateSettings("dbgmenu, Enable Fog", "dbgmenu, Enable Fog, 0");
                    UpdateSettings("dbgmenu, Enable vFog", "dbgmenu, Enable vFog, 1");
                    UpdateRegistry("Fog", "Close Fog Only");
                    break;

                case 3:
                    UpdateSettings("dbgmenu, Enable Fog", "dbgmenu, Enable Fog, 0");
                    UpdateSettings("dbgmenu, Enable vFog", "dbgmenu, Enable vFog, 0");
                    UpdateRegistry("Fog", "Close Fog + Distant Fog");
                    break;
            }
        }
        private void comboBox_CalculateSplineMesh_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_CalculateSplineMesh.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Anim Spline Mesh generation mode", "dbgmenu, Anim Spline Mesh generation mode, Mesh generation on GPU");
                    UpdateRegistry("Calculate Spline Mesh Device", "GPU");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Anim Spline Mesh generation mode", "dbgmenu, Anim Spline Mesh generation mode, Mesh generation on CPU");
                    UpdateRegistry("Calculate Spline Mesh Device", "CPU");
                    break;
            }
        }
        private void comboBox_PauseWorldMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_PauseWorldMap.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Map Uses Impostors", "dbgmenu, Map Uses Impostors, 0");
                    UpdateSettings("dbgmenu, Load Geo Zone", "dbgmenu, Load Geo Zone, 0");
                    UpdateRegistry("Pause World Map", "Default");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Map Uses Impostors", "dbgmenu, Map Uses Impostors, 1");
                    UpdateSettings("dbgmenu, Load Geo Zone", "dbgmenu, Load Geo Zone, 1");
                    UpdateRegistry("Pause World Map", "Realistic");
                    break;
            }
        }
        private void comboBox_ColorCorrection_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_ColorCorrection.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Color Correction", "dbgmenu, Enable Color Correction, 1");
                    UpdateRegistry("Color Correction", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Color Correction", "dbgmenu, Enable Color Correction, 0");
                    UpdateRegistry("Color Correction", "Enabled");
                    break;
            }
        }
        private void comboBox_ChromaticAberration_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_ChromaticAberration.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Chromatic Aberration", "dbgmenu, Enable Chromatic Aberration, 1");
                    UpdateRegistry("Chromatic Aberration", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Chromatic Aberration", "dbgmenu, Enable Chromatic Aberration, 0");
                    UpdateRegistry("Chromatic Aberration", "Enabled");
                    break;
            }
        }
        private void comboBox_FilmGrain_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_FilmGrain.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Film Grain", "dbgmenu, Enable Film Grain, 1");
                    UpdateRegistry("Film Grain", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Film Grain", "dbgmenu, Enable Film Grain, 0");
                    UpdateRegistry("Film Grain", "Enabled");
                    break;
            }
        }
        private void comboBox_MotionBlur_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_MotionBlur.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Motion Blur", "dbgmenu, Enable Motion Blur, 0");
                    UpdateRegistry("Motion Blur", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Motion Blur", "dbgmenu, Enable Motion Blur, 1");
                    UpdateRegistry("Motion Blur", "Enabled");
                    break;
            }
        }
        private void comboBox_DepthOfField_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_DepthOfField.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Depth-of-Field", "dbgmenu, Enable Depth-of-Field, 0");
                    UpdateSettings("dbgmenu, Enable Depth Of Field", "dbgmenu, Enable Depth Of Field, 1");
                    UpdateRegistry("Depth Of Field", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Depth-of-Field", "dbgmenu, Enable Depth-of-Field, 1");
                    UpdateSettings("dbgmenu, Enable Depth Of Field", "dbgmenu, Enable Depth Of Field, 0");
                    UpdateRegistry("Depth Of Field", "Enabled");
                    break;
            }
        }
        private void comboBox_Bloom_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Bloom.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Bloom", "dbgmenu, Enable Bloom, 1");
                    UpdateRegistry("Bloom", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Bloom", "dbgmenu, Enable Bloom, 0");
                    UpdateRegistry("Bloom", "Enabled");
                    break;
            }
        }








        private void comboBox_Ansel_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Ansel.SelectedIndex)
            {
                case 0:
                    _gameAnselExecutable = false;
                    UpdateRegistry("Ansel", "Disabled");

                    label_AnselWarning.Visible = false;
                    break;

                case 1:
                    _gameAnselExecutable = true;
                    UpdateRegistry("Ansel", "Enabled");

                    label_AnselWarning.Visible = true;
                    break;
            }
        }
        private void comboBox_WorldStreaming_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_WorldStreaming.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, ZoneInstantiationBudgetMin", string.Empty);
                    UpdateSettings("dbgmenu, ZoneInstantiationBudgetMax", string.Empty);
                    UpdateSettings("dbgmenu, AssetInstantiationBudgetMin", string.Empty);
                    UpdateSettings("dbgmenu, AssetInstantiationBudgetMax", string.Empty);
                    UpdateSettings("dbgmenu, AssetDeletionBudgetMin", string.Empty);
                    UpdateSettings("dbgmenu, AssetDeletionBudgetMax", string.Empty);
                    UpdateRegistry("World Streaming", "Default");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, ZoneInstantiationBudgetMin", "dbgmenu, ZoneInstantiationBudgetMin, 0.0120");
                    UpdateSettings("dbgmenu, ZoneInstantiationBudgetMax", "dbgmenu, ZoneInstantiationBudgetMax, 0.0200");
                    UpdateSettings("dbgmenu, AssetInstantiationBudgetMin", "dbgmenu, AssetInstantiationBudgetMin, 0.0130");
                    UpdateSettings("dbgmenu, AssetInstantiationBudgetMax", "dbgmenu, AssetInstantiationBudgetMax, 0.0200");
                    UpdateSettings("dbgmenu, AssetDeletionBudgetMin", "dbgmenu, AssetDeletionBudgetMin, 0.0115");
                    UpdateSettings("dbgmenu, AssetDeletionBudgetMax", "dbgmenu, AssetDeletionBudgetMax, 0.0200");
                    UpdateRegistry("World Streaming", "High Budget");
                    break;
            }
        }
        private void comboBox_DelayedStreaming_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_DelayedStreaming.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Delayed Streaming", "dbgmenu, Enable Delayed Streaming, 0");
                    UpdateRegistry("Delayed Streaming", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Delayed Streaming", "dbgmenu, Enable Delayed Streaming, 1");
                    UpdateRegistry("Delayed Streaming", "Enabled");
                    break;
            }
        }
        private void comboBox_CompositeMaterials_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_CompositeMaterials.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Bypass Composite Materials", "dbgmenu, Bypass Composite Materials, 1");
                    UpdateRegistry("Composite Materials", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Bypass Composite Materials", "dbgmenu, Bypass Composite Materials, 0");
                    UpdateRegistry("Composite Materials", "Enabled");
                    break;
            }
        }
        private void comboBox_Outlines_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Outlines.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Bypass Outlines", "dbgmenu, Bypass Outlines, true");
                    UpdateRegistry("Outlines", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Bypass Outlines", "dbgmenu, Bypass Outlines, false");
                    UpdateRegistry("Outlines", "Enabled");
                    break;
            }
        }
        private void comboBox_VFX_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_VFX.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable VFX", "dbgmenu, Enable VFX, false");
                    UpdateRegistry("VFX", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable VFX", "dbgmenu, Enable VFX, true");
                    UpdateRegistry("VFX", "Enabled");
                    break;
            }
        }
        private void comboBox_Decals_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Decals.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Decals", "dbgmenu, Enable Decals, false");
                    UpdateRegistry("Decals", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Decals", "dbgmenu, Enable Decals, true");
                    UpdateRegistry("Decals", "Enabled");
                    break;
            }
        }
        private void comboBox_Weather_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Weather.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Enable Weather", "dbgmenu, Enable Weather, 0");
                    UpdateRegistry("Weather", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Enable Weather", "dbgmenu, Enable Weather, 1");
                    UpdateRegistry("Weather", "Enabled");
                    break;
            }
        }
        private void comboBox_StartupIntro_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_StartupIntro.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-legal", string.Empty);
                    UpdateRegistry("Startup Intro", "Disabled");
                    break;

                case 1:
                    UpdatePreciseStartupArguments("-legal", "-legal");
                    UpdateRegistry("Startup Intro", "Enabled");
                    break;
            }
        }
        private void comboBox_MainMenu_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_MainMenu.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-lobby", string.Empty);
                    UpdateRegistry("Main Menu", "Disabled");
                    break;

                case 1:
                    UpdatePreciseStartupArguments("-lobby", "-lobby");
                    UpdateRegistry("Main Menu", "Enabled");
                    break;
            }
        }
        private void comboBox_Godmode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Godmode.SelectedIndex)
            {
                case 0:
                    UpdatePreciseStartupArguments("-godmode", string.Empty);
                    UpdateRegistry("Godmode", "Disabled");
                    break;

                case 1:
                    UpdatePreciseStartupArguments("-godmode", "-godmode");
                    UpdateRegistry("Godmode", "Enabled");
                    break;
            }
        }
        private void comboBox_OperatorMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_OperatorMode.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Timecode Mode", "dbgmenu, Timecode Mode, kDisabled");
                    UpdateRegistry("Operator Mode", "Disabled");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Timecode Mode", "dbgmenu, Timecode Mode, kEnabledCine");
                    UpdateRegistry("Operator Mode", "Enabled");
                    break;
            }
        }
        private void comboBox_CaptureFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_CaptureFormat.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Capture Image Format", "dbgmenu, Capture Image Format, bmp");
                    UpdateRegistry("Capture Format", "bmp");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Capture Image Format", "dbgmenu, Capture Image Format, dds");
                    UpdateRegistry("Capture Format", "dds");
                    break;

                case 2:
                    UpdateSettings("dbgmenu, Capture Image Format", "dbgmenu, Capture Image Format, jpg");
                    UpdateRegistry("Capture Format", "jpg");
                    break;

                case 3:
                    UpdateSettings("dbgmenu, Capture Image Format", "dbgmenu, Capture Image Format, png");
                    UpdateRegistry("Capture Format", "png");
                    break;

                case 4:
                    UpdateSettings("dbgmenu, Capture Image Format", "dbgmenu, Capture Image Format, hdr");
                    UpdateRegistry("Capture Format", "hdr");
                    break;
            }
        }
        private void comboBox_CaptureResolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_CaptureResolution.SelectedIndex)
            {
                case 0:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 0");
                    UpdateRegistry("Capture Resolution", "Default");
                    break;

                case 1:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 1");
                    UpdateRegistry("Capture Resolution", "256 Square");
                    break;

                case 2:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 2");
                    UpdateRegistry("Capture Resolution", "512 Square");
                    break;

                case 3:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 3");
                    UpdateRegistry("Capture Resolution", "1080p");
                    break;

                case 4:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 4");
                    UpdateRegistry("Capture Resolution", "1440p");
                    break;

                case 5:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 5");
                    UpdateRegistry("Capture Resolution", "4K");
                    break;

                case 6:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 6");
                    UpdateRegistry("Capture Resolution", "8K");
                    break;

                case 7:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 7");
                    UpdateRegistry("Capture Resolution", "8192 Square");
                    break;

                case 8:
                    UpdateSettings("dbgmenu, Resolved Capture Resolution", "dbgmenu, Resolved Capture Resolution, 8");
                    UpdateRegistry("Capture Resolution", "10K");
                    break;
            }
        }








        private void comboBox_Experiment_DiscordRPC_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Experiment_DiscordRPC.SelectedIndex)
            {
                case 0:
                    SetExperimentState(_experiment_DiscordRPC, E_ExperimentState.DISABLED);
                    break;

                case 1:
                    SetExperimentState(_experiment_DiscordRPC, E_ExperimentState.ENABLED);
                    break;
            }
        }
        private void comboBox_Experiment_NoCrashWindow_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox_Experiment_NoCrashWindow.SelectedIndex)
            {
                case 0:
                    SetExperimentState(_experiment_NoCrashWindow, E_ExperimentState.DISABLED);
                    break;

                case 1:
                    SetExperimentState(_experiment_NoCrashWindow, E_ExperimentState.ENABLED);
                    break;
            }
        }
        private async void comboBox_Experiment_CompressGameFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            label_Experiment_CompressGameFilesToolTip.Visible = comboBox_Experiment_CompressGameFiles.SelectedIndex == 1;

            if (_settingsInitialized)
            {
                DialogResult dialogResult = MessageBox.Show("Games files compression / uncompression is a heavy process that can take up to 4 hours to perform...\n\nDo you want to continue?", "Wolverine Compression", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    comboBox_Experiment_CompressGameFiles.Enabled = false;
                    _compressionProgress.Show();
                    switch (comboBox_Experiment_CompressGameFiles.SelectedIndex)
                    {
                        case 0:
                            _compressionProgress.Start(false, compressionFilesList);
                            await Task.Run(() =>
                            {
                                SetCompressionSettings(false);
                            });
                            break;

                        case 1:
                            _compressionProgress.Start(true, compressionFilesList);
                            await Task.Run(() =>
                            {
                                SetCompressionSettings(true);
                            });
                            break;
                    }

                    comboBox_Experiment_CompressGameFiles.Enabled = true;
                    string friendlyState = GetCompressionSettings() ? "Enabled" : "Disabled";

                    if (_compressionProgress.Visible)
                        _compressionProgress.Stop();

                    MessageBox.Show($"New compression state: {friendlyState}", "Wolverine Compression", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    _settingsInitialized = false;

                    if (GetCompressionSettings() == false)
                        comboBox_Experiment_CompressGameFiles.SelectedIndex = 0;
                    else
                        comboBox_Experiment_CompressGameFiles.SelectedIndex = 1;

                    _settingsInitialized = true;
                }
            }
        }   








        private void textBox_CustomGameStartupArguments_TextChanged(object sender, EventArgs e)
        {
            _customGameStartupArguments = textBox_CustomGameStartupArguments.Text;
            UpdateRegistry("Custom Game Startup Arguments", textBox_CustomGameStartupArguments.Text);
        }








        private void button_OK_MouseClick(object sender, MouseEventArgs e) => this.Hide();
        private void checkBox_DefaultSettings_CheckedChanged(object sender, EventArgs e) => button_DefaultSettings.Enabled = checkBox_DefaultSettings.Checked;
        private void button_DefaultSettings_MouseClick(object sender, MouseEventArgs e)
        {
            WinReg.DestroySubKey();

            if (Constants._timePlayed > 0)
            {
                WinReg.SetData_DWORD("Time Played", Constants._timePlayed);
            }
            InitializeSettings();

            checkBox_DefaultSettings.Checked = false;
        }
        private void button_SaveGameDirectory_MouseClick(object sender, MouseEventArgs e) => Process.Start(Constants._saveGameDirectory);








        private void linkLabel_Ansel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://www.nvidia.com/en-us/geforce/geforce-experience/ansel");
        private void linkLabel_Ansel_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "https://www.nvidia.com/en-us/geforce/geforce-experience/ansel";
            toolTip_Info.Show(toolTipText, linkLabel_Ansel, 30000);
        }








        // HINTS
        private void label_DebugControlsHelp_MouseHover(object sender, EventArgs e) => _debugControlsHint.Show();
        private void label_DebugControlsHelp_MouseLeave(object sender, EventArgs e) => _debugControlsHint.Hide();



        private void label_AntiAliasingHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_AntiAliasing);
        private void label_AntiAliasingHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_UpscalingHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_Upscaling);
        private void label_UpscalingHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_ColorCorrectionHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_ColorCorrection);
        private void label_ColorCorrectionHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_ChromaticAberrationHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_ChromaticAberration);
        private void label_ChromaticAberrationHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_FilmGrainHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_FilmGrain);
        private void label_FilmGrainHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_MotionBlurHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_MotionBlur);
        private void label_MotionBlurHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_DepthOfFieldHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_DepthOfField);
        private void label_DepthOfFieldHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_BloomHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_Bloom);
        private void label_BloomHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_GeometryQualityHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_GeometryQuality);
        private void label_GeometryQualityHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_IKFootHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_IKFoot);
        private void label_IKFootHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_ShadowQualityHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_ShadowQuality);
        private void label_ShadowQualityHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_AmbientOcclusionHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_AmbientOcclusion);
        private void label_AmbientOcclusionHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_ScreenSpaceReflectionsHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_ScreenSpaceReflections);
        private void label_ScreenSpaceReflectionsHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_CloudsQualityHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_CloudsQuality);
        private void label_CloudsQualityHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_FogHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_FogQuality);
        private void label_FogHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_CalculateSplineMeshHelp_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_CalculateSplineMesh);
        private void label_CalculateSplineMeshHelp_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_PauseWorldMapHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_PauseWorldMap);
        private void label_PauseWorldMapHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_AnselHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_Ansel);
        private void label_AnselHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_StartupIntroHelp_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_StartupIntro);
        private void label_StartupIntroHelp_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_MainMenuHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_MainMenu);
        private void label_MainMenuHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_OperatorModeHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_OperatorMode);
        private void label_OperatorModeHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_CompositeMaterialsHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_CompositeMaterials);
        private void label_CompositeMaterialsHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_VFXHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_VFX);
        private void label_VFXHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_DecalsHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_Decals);
        private void label_DecalsHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_WeatherHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_Weather);
        private void label_WeatherHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);



        private void label_Experiment_DiscordRPCHint_MouseHover(object sender, EventArgs e) => UpdateHintState(E_HintState.VISIBLE, Properties.Resources.Hint_DiscordRPC);
        private void label_Experiment_DiscordRPCHint_MouseLeave(object sender, EventArgs e) => UpdateHintState(E_HintState.HIDDEN);








        private async void Handle_LanguageLabelAnimation()
        {
            string regionalString = string.Empty;
            switch (Constants._cultureTwoLetterCode)
            {
                case "en":
                    return;

                case "es":
                    regionalString = "Idioma";
                    break;

                case "zh":
                    regionalString = "语言";
                    break;

                case "ar":
                    regionalString = "لغة";
                    break;

                case "pt":
                    regionalString = "Língua";
                    break;

                case "ru":
                    regionalString = "Язык";
                    break;

                case "ja":
                    regionalString = "言語";
                    break;

                case "de":
                    regionalString = "Sprache";
                    break;

                case "fr":
                    regionalString = "Langue";
                    break;

                case "ko":
                    regionalString = "언어";
                    break;

                case "it":
                    regionalString = "Lingua";
                    break;

                case "nl":
                    regionalString = "Taal";
                    break;

                case "sv":
                    regionalString = "Språk";
                    break;

                case "pl":
                    regionalString = "Język";
                    break;

                case "no":
                    regionalString = "Språk";
                    break;

                case "da":
                    regionalString = "Sprog";
                    break;

                case "fi":
                    regionalString = "Kieli";
                    break;

                case "cs":
                    regionalString = "Jazyk";
                    break;

                case "hu":
                    regionalString = "Nyelv";
                    break;

                case "el":
                    regionalString = "Γλώσσα";
                    break;

                default:
                    return;
            }



            string[] localizationText = { label_Language.Text, regionalString};
            int currentIndex = 1;
            while (true)
            {
                await Task.Delay(5000);

                string currentText = localizationText[currentIndex];
                int length = currentText.Length + 1;
                for (int i = 0; i < length; i++)
                {
                    label_Language.Text = currentText.Substring(0, i) + new String(' ', Math.Max(0, length - i));
                    await Task.Delay(100);
                }

                currentIndex = (currentIndex + 1) % localizationText.Length;
            }
        }








        private void label_PerformanceStatsWarning_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Performance Stats are part of game Debug functionality, they can't be seen while \"Debug Mode\" is set to \"Minimum\".";
            toolTip_Warning.Show(toolTipText, label_PerformanceStatsWarning, 30000);
        }
        private void label_AspectRatioWarning_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Game was never designed to work properly with any aspect ratio other than default one (16:9).";
            toolTip_Warning.Show(toolTipText, label_AspectRatioWarning, 30000);
        }
        private void label_DebugModeWarning_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Developer Menu (ESC) can not be accessed while \"Debug Mode\" is set to \"Minimum\".";
            toolTip_Warning.Show(toolTipText, label_DebugModeWarning, 30000);
        }
        private void label_ControllerSpeakerWarning_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Playstation 5 controller (Dual Sense) haptic feedback will not work properly w/o this feature enabled.";
            toolTip_Warning.Show(toolTipText, label_ControllerSpeakerWarning, 30000);
        }
        private void label_UpscalingWarning_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Upscaling feature relies on Temportal Anti-Aliasing (TAA) & won't work w/o it enabled.";
            toolTip_Warning.Show(toolTipText, label_UpscalingWarning, 30000);
        }
        private void label_UpscalingToolTip_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Rendering game at lower resolution, upscaling algorithms are trying to predict and generate additional pixels to enhance\nthe visual fidelity and maintain smoothness, ensuring an immersive gaming experience even at lower resolutions.";
            toolTip_Info.Show(toolTipText, label_UpscalingToolTip, 30000);
        }
        private void label_HardDriveWarning_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Although technically game can be run on HDD, we strongly recommend transferring it to an SSD instead.\nSlow storage unit might result serious, gameplay related glitches that will only make your gaming experience worse.";
            toolTip_Warning.Show(toolTipText, label_HardDriveWarning, 30000);
        }
        private void label_FrameRateCapToolTip_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "Due to Console Gaming specifics, 120FPS mode was designed the way it lowers graphics settings undepending on user preferences.";
            toolTip_Info.Show(toolTipText, label_FrameRateCapToolTip, 30000);
        }
        private void label_AnselWarning_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "In order to utilize NVIDIA Ansel, game will be launching through \"Remnant-Win64-Shipping.exe\" instead of its default executable.\nThis action will trick NVIDIA to believe that we're playing a game with full Ansel support.";
            toolTip_Warning.Show(toolTipText, label_AnselWarning, 30000);
        }
        private void label_Experiment_CompressGameFilesToolTip_MouseHover(object sender, EventArgs e)
        {
            string toolTipText = "When game is compressed, storage it's running on will be utilized less and will improve situations, where Open World & Cutscenes doesn't load in time, which usually leads to\ncatastrophic outcomes. Althrough it seems to be 100% effective solution, it will also increase CPU utilization due to fact PC is now decompressing data first.";
            toolTip_Info.Show(toolTipText, label_Experiment_CompressGameFilesToolTip, 30000);
        }








        private void checkBox_Warning_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Warning.Checked)
            {
                checkBox_Warning.Enabled = false;

                comboBox_Godmode.Enabled = true;

                comboBox_OperatorMode.Enabled = true;
                comboBox_CaptureFormat.Enabled = true;
                comboBox_CaptureResolution.Enabled = true;

                comboBox_WorldStreaming.Enabled = true;
                comboBox_DelayedStreaming.Enabled = true;

                comboBox_CompositeMaterials.Enabled = true;
                comboBox_Decals.Enabled = true;

                comboBox_Weather.Enabled = true;
                comboBox_VFX.Enabled = true;
                comboBox_Outlines.Enabled = true;
            }
        }
    }
}