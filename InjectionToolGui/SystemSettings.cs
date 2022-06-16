using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Management;

namespace InjectionToolGui
{
    /// <summary>
    /// Class for checking required system settings
    /// </summary>
    internal static class SystemSettings
    {
        public static bool SecureBoot;
        public static bool TPM;
        public static bool Network;
        public static bool CoreIsolation;

        /// <summary>
        /// Verify network, secure boot, tpm, and core isolation.
        /// </summary>
        public static void CheckAllSettings()
        {
            CheckNetwork();
            CheckSecureBoot();
            CheckTPM();
            CheckCoreIsolation();
        }

        /// <summary>
        /// Verify that secure boot is enabled and functioning
        /// </summary>
        /// <returns><see cref="bool"/><para>True if enabled, false if not.</para></returns>
        /// <exception cref="Exception"></exception>
        public static bool CheckSecureBoot()
        {
            int state = 0;
            string key = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecureBoot\State";
            string subkey = @"UEFISecureBootEnabled";
            try
            {
                object? value = Registry.GetValue(key, subkey, state);
                if (value != null)
                {
                    state = (int)value;
                    if (state == 1)
                    {
                        SecureBoot = true;
                        return SecureBoot;
                    }
                    else
                    {
                        SecureBoot = false;
                        return SecureBoot;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                SecureBoot = false;
                return SecureBoot;
            }

            throw new Exception("Failed to check secure boot.");
        }

        /// <summary>
        /// Verify that TPM is enabled.
        /// </summary>
        /// <returns><see cref="bool"/><para>True if enabled, false if disabled.</para></returns>
        public static bool CheckTPM()
        {
            ManagementObjectSearcher tpmSearcher = new("root\\CIMV2\\Security\\MicrosoftTpm",
                "SELECT * FROM Win32_Tpm");
            foreach (ManagementObject tpm in tpmSearcher.Get())
            {
                TPM = (bool)tpm["IsEnabled_InitialValue"];
            }

            return TPM;
        }

        /// <summary>
        /// Sends ping to verify local network connection.
        /// </summary>
        /// <returns><see cref="bool"/><para>True if network ping gets a response, and false if not.</para></returns>
        public static bool CheckNetwork()
        {
            try
            {
                System.Net.NetworkInformation.Ping ping = new();
                System.Net.NetworkInformation.PingReply pingReply = ping.Send("192.168.120.17");


                if (pingReply != null)
                {
                    Network = true;
                }
                else
                {
                    Network = false;
                }

                return Network;
            }
            catch (System.Net.NetworkInformation.PingException ex)
            {
                Debug.WriteLine(ex);

                return false;
            }
        }

        /// <summary>
        /// Verify that Core Isolation is enabled.
        /// </summary>
        /// <returns><see cref="bool"/> representing whether core isolation is enabled.</returns>
        /// <exception cref="Exception"></exception>
        public static bool CheckCoreIsolation()
        {
            int coreState = 0;
            string coreKey = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity";
            string coreSubkey = @"Enabled";
            try
            {
                object? value = Registry.GetValue(coreKey, coreSubkey, coreState);
                if (value != null)
                {
                    coreState = (int)value;
                    if (coreState == 1)
                    {
                        CoreIsolation = true;
                        return CoreIsolation;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                CoreIsolation = false;
                return CoreIsolation;
            }

            throw new Exception("Failed to check core isolation.");
        }
    }
}
