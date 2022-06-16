using System.IO;

namespace InjectionToolGui
{
    /// <summary>
    /// Class to count total storage and check if it meets the advanced key parameter.
    /// </summary>
    internal class Storage
    {
        public uint TotalStorageSpace { get; set; }
        public bool Advanced { get; set; }

        public Storage()
        {
            TotalStorageSpace = FindTotalStorageSpace();
            Advanced = FindAdvanced();
        }

        private uint FindTotalStorageSpace()
        {
            ulong totalSizeInBytes = 0;

            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    totalSizeInBytes += (ulong) drive.TotalSize;
                }
            }

            return (uint)(totalSizeInBytes / (1024 * 1024 * 1024));

        }

        private bool FindAdvanced()
        {
            if (TotalStorageSpace > 1250)
            {
                return true;
            }

            return false;
        }
    }
}
