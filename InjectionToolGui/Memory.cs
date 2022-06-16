using System;
using System.Management;

namespace InjectionToolGui
{
    internal class Memory
    {
        public uint Size { get; set; }
        public bool Advanced { get; set; }

        public Memory()
        {
            Size = FindMemorySize();
            Advanced = FindAdvanced();
        }

        private uint FindMemorySize()
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
