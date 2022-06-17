using System.IO;

namespace InjectionToolGui
{
    /// <summary>
    /// Represents the storage info needed for injection.
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

        /// <summary>
        /// Finds the total storage space in the system.
        /// </summary>
        /// <returns><see cref="uint"/> representing the total storage space in gigabytes</returns>
        private static uint FindTotalStorageSpace()
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

        /// <summary>
        /// Checks if the total storage space meets the requirements for advanced keys.
        /// </summary>
        /// <returns><see cref="bool"/> representing the requirements for an advanced key is met.</returns>
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
