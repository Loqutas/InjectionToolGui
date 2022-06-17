using Microsoft.Win32;
using System;
using System.Management;
using System.Windows;

namespace InjectionToolGui
{
    /// <summary>
    /// Represents the operating system info needed for injectiion.
    /// </summary>
    internal class OS
    {
        /// <summary>
        /// <see cref="Enum"/> Options for installed Windows editions.
        ///     Only contains editions that are used by the key server.
        /// </summary>
        public enum Editions { NONE, Home, HomeA, Pro, ProA, RDPK }

        public string Version { get; set; }
        public Editions Edition { get; set; }
        public string Full { get; set; }

        public OS()
        {
            Version = FindVersion();
            Edition = FindEdition();
            Full = $"Windows {Version} {Edition}";
        }

        /// <summary>
        /// Finds the installed Windows version.
        /// </summary>
        /// <returns><see cref="string"/> representing the installed Windows version.</returns>
        private string FindVersion()
        {
            try
            {
                ManagementObjectSearcher windowsVersion = new(
                "root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");
                foreach (ManagementObject windows in windowsVersion.Get())
                {

                    string buildString = (string)windows["BuildNumber"];
                    uint buildNum = Convert.ToUInt32(buildString);

                    if (buildNum >= 22000)
                    {
                        Version = "11";
                    }
                    else
                    {
                        Version = "10";
                    }

                    return Version;

                }

                throw new Exception("Could not find Windows Version.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                MessageBox.Show(e.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Finds the installed windows edition.
        /// </summary>
        /// <returns><see cref="OS.Editions"/> representing the installed Windows edition.</returns>
        private static Editions FindEdition()
        {
            try
            {
                const string key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
                const string subKey = @"EditionID";
                string? winEdition = string.Empty;
                object? keyValue = Registry.GetValue(key, subKey, winEdition);

                if (keyValue != null)
                {
                    winEdition = keyValue.ToString();

                    return winEdition switch
                    {
                        string edition when edition.Contains("Core") => Editions.Home,
                        string edition when edition.Contains("Pro") => Editions.Pro,
                        _ => Editions.NONE,
                    };
                }

                throw new Exception("Could not find Windows Edition.");
            }
            catch (Exception e)
            {
                MessageBox.Show("Failure to read edition.");
                Console.WriteLine(e);
                return Editions.NONE;
            }
        }
    }
}
