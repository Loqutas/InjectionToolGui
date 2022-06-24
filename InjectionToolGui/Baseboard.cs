using System;
using System.Management;

namespace InjectionToolGui
{
    /// <summary>
    /// Represents the baseboard info needed for injection.
    /// </summary>
    internal class Baseboard
    {
        public enum MANUFACTURER { NONE, ASROCK, ASUS, ASUSZ690, GIGABYTE, MSI, SAGER, SAGERH2O, TONGFANG }

        public MANUFACTURER Manufacturer { get; set; }
        public string? Name { get; set; }

        public Baseboard()
        {
            Name = FindBaseboardInfo();
            Manufacturer = FindManufacturer();
        }

        private MANUFACTURER FindManufacturer()
        {
            ManagementObjectSearcher searcher = new(@"root\cimv2", "SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject obj in searcher.Get())
            {
                string manufacturer = (string) obj["Manufacturer"];
                Manufacturer = manufacturer switch
                {
                    string man when man.Contains("ASRock") => MANUFACTURER.ASROCK,
                    string man when man.Contains("ASUS") => MANUFACTURER.ASUS,
                    string man when man.Contains("Gigabyte") => MANUFACTURER.GIGABYTE,
                    string man when man.Contains("Micro") => MANUFACTURER.MSI,
                    string man when man.Contains("Notebook") => MANUFACTURER.SAGERH2O,
                    string man when man.Contains("THTF") => MANUFACTURER.TONGFANG,
                    _ => MANUFACTURER.NONE,
                };
            }

            return Manufacturer;
        }

        private string? FindBaseboardInfo()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\cimv2", "SELECT * FROM Win32_BaseBoard");
            foreach (ManagementObject obj in searcher.Get())
            {
                Name = (string)obj["Product"];
                Name = Name.Replace(" ", "");
                Name = Name.Replace(",", "");
                Name = Name.Replace("/", "");
                Name = Name.Replace(@"\", "");

                if (Name.Contains('('))
                {
                    Name = Name[..Name.IndexOf("(")];
                }

                return Name;
            }

            return null;
        }
    }
}
