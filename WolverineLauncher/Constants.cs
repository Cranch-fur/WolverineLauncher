using System;
using System.Globalization;
using System.IO;

namespace WolverineLauncher
{
    public static class Constants
    {
        public static readonly CultureInfo _culture = CultureInfo.CurrentCulture;
        public static readonly string _cultureTwoLetterCode = _culture.TwoLetterISOLanguageName;

        public static readonly int _timePlayed = WinReg.GetData_DWORD("Time Played");

        public static readonly string _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string _versionPath = Path.Combine(_currentDirectory, "version.txt");

        public static readonly string _workspaceDirectory = Path.Combine(_currentDirectory, "workspace");
        public static readonly string _gameFilesDirectory = Path.Combine(_workspaceDirectory, "d");
        public static readonly string _pluginsDirectory = Path.Combine(_workspaceDirectory, "plugins");

        public static readonly string _gameExecutablePath = Path.Combine(_workspaceDirectory, "Marvel's Wolverine.exe");
        public static readonly string _anselGameExecutablePath = Path.Combine(_workspaceDirectory, "Remnant-Win64-Shipping.exe");

        public static readonly string _settingsPath = Path.Combine(_workspaceDirectory, "cmd.txt");
        public static readonly string _saveGameDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".insomniac", "InsomniacEngine");
    }
}
