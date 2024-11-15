using Microsoft.Win32;
using System;


namespace WolverineLauncher
{
    public static class WinReg
    {
        // BINARY = byte[]
        // DWORD = int (Int32)
        // QWORD = long (Int64)
        // SZ = string

        // "HKEY_CURRENT_USER" --> Hive
        // "HKEY_CURRENT_USER\SOFTWARE" --> Key
        // "HKEY_CURRENT_USER\SOFTWARE\Microsoft" --> Sub-Key
        // "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer" --> Sub-Key
        // "HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\Start Page (REG_SZ)" --> Value
        public const string _softwareSubKeyPath = @"HKEY_CURRENT_USER\SOFTWARE\Marvel's Wolverine";
        private static readonly string _softwareShortSubKeyPath = _softwareSubKeyPath.Substring(_softwareSubKeyPath.IndexOf('\\') + 1);

        public const string _themesSubKeyPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes";






        public static bool GetSubKeyExist()
        {
            try
            {
                using (RegistryKey subKey = Registry.CurrentUser.OpenSubKey(_softwareShortSubKeyPath, true))
                {
                    if (subKey != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch 
            { 
                return false; 
            }
        }
        public static bool DestroySubKey()
        {
            try
            {
                using (RegistryKey subKey = Registry.CurrentUser.OpenSubKey(_softwareShortSubKeyPath, true))
                {
                    if (subKey != null)
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(_softwareShortSubKeyPath);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch 
            {
                return false; 
            }
        }




        public static byte[] GetData_BINARY(string entryName, byte[] defaultReturn = null)
        {
            object data = Registry.GetValue(_softwareSubKeyPath, entryName, defaultReturn);
            if (data != null && data is byte[])
            {
                return (byte[])data;
            }
            else
                return defaultReturn;
        }
        public static int GetData_DWORD(string entryName, int defaultReturn = -1)
        {
            object data = Registry.GetValue(_softwareSubKeyPath, entryName, defaultReturn);
            if (data != null && data is int)
            {
                return Convert.ToInt32(data);
            }
            else
                return defaultReturn;
        }
        public static long GetData_QWORD(string entryName, long defaultReturn = -1)
        {
            object data = Registry.GetValue(_softwareSubKeyPath, entryName, defaultReturn);
            if (data != null && data is long)
            {
                return Convert.ToInt32(data);
            }
            else
                return defaultReturn;
        }
        public static string GetData_SZ(string entryName, string defaultReturn = null)
        {
            object data = Registry.GetValue(_softwareSubKeyPath, entryName, defaultReturn);
            if (data != null && data is string)
            {
                return Convert.ToString(data);
            }
            else
                return defaultReturn;
        }




        public static bool SetData_BINARY(string entryName, byte[] data)
        {
            try
            {
                Registry.SetValue(_softwareSubKeyPath, entryName, data, RegistryValueKind.Binary);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetData_DWORD(string entryName, int data)
        {
            try
            {
                Registry.SetValue(_softwareSubKeyPath, entryName, data, RegistryValueKind.DWord); 
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetData_QWORD(string entryName, long data)
        {
            try
            {
                Registry.SetValue(_softwareSubKeyPath, entryName, data, RegistryValueKind.QWord);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SetData_SZ(string entryName, string data)
        {
            try
            {
                Registry.SetValue(_softwareSubKeyPath, entryName, data, RegistryValueKind.String);
                return true;
            }
            catch
            {
                return false;
            }
        }




        public static bool GetDarkThemeEnabled()
        {
            try
            {
                object data = Registry.GetValue($@"{_themesSubKeyPath}\Personalize", "AppsUseLightTheme", -1);
                if (data != null && data is int)
                {
                    bool isDarkTheme = (int)data == 0;
                    return isDarkTheme;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
