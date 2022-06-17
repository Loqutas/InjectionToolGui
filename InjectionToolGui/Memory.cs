using System;
using System.Management;

namespace InjectionToolGui
{
    /// <summary>
    /// Represents the memory info needed for injection.
    /// </summary>
    internal class Memory
    {
        public uint Size { get; set; }
        public bool Advanced { get; set; }

        public Memory()
        {
            Size = FindMemorySize();
            Advanced = FindAdvanced();
        }

        /// <summary>
        /// Finds the size of the installed memory.
        /// </summary>
        /// <returns><see cref="uint"/> representing the total memory in gigabytes.</returns>
        private static uint FindMemorySize()
        {
            double size = 0;

            ManagementObjectSearcher searcher = new(@"root\cimv2", "SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                UInt64 sizeInBytes = (UInt64) obj["TotalPhysicalMemory"];

                size = (double)Math.Ceiling(sizeInBytes / (1024.0 * 1024.0 * 1024.0));
            }

            return (uint) size;
        }

        /// <summary>
        /// Checks if the installed memory meets the requirements for an advanced key.
        /// </summary>
        /// <returns><see cref="bool"/> representing if the memory meets the requirements for an advanced key.</returns>
        private bool FindAdvanced()
        {
            if (Size > 8)
            {
                return true;
            }

            return false;
        }
    }
}
