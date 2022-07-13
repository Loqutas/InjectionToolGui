using System.Text;
using System.Management;

namespace InjectionToolGui

{
    /// <summary>
    /// Represents the installed processor info needed for injection.
    /// </summary>
    internal class Processor
    {
        public string Name { get; }
        public bool Advanced { get; }

        public Processor()
        {
            Name = FindProcessorName();
            Advanced = FindAdvanced();
        }

        /// <summary>
        /// Finds the processor name.
        /// </summary>
        /// <returns><see cref="StringBuilder.ToString()"/> representing the processor name.</returns>
        private string FindProcessorName()
        {
            StringBuilder stringBuilder = new();

            ManagementObjectSearcher searcher = new(@"root\cimv2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                stringBuilder.Append(obj[nameof(Name)].ToString());
            }

            return stringBuilder.ToString().Contains('@') ? 
                stringBuilder.ToString().Remove(stringBuilder.ToString().IndexOf('@')) :
                stringBuilder.ToString();
        }

        /// <summary>
        /// Checks if the processor meets the requirements for an advanced key.
        /// </summary>
        /// <returns><seealso cref="bool"/></returns>
        private bool FindAdvanced()
        {
            if (Name != null)
            {
                return Name switch
                {
                    string name when name.Contains("i9") => true,
                    string name when name.Contains("Ryzen 9") => true,
                    string name when name.Contains("i7") => true,
                    string name when name.Contains("Ryzen 7") => true,
                    string name when name.Contains("Threadripper") => true,
                    _ => false,
                };
            }

            return false;
        }
    }
}
