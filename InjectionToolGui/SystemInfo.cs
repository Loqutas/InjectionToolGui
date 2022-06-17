using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InjectionToolGui
{
    /// <summary>
    /// Represents all the system info needed for injection
    /// </summary>
    internal class SystemInfo : INotifyPropertyChanged
    {
        private string? productId;
        private string? orderId;
        private string? activationKey;
        private bool useTestKey;
        private bool interactive;
        private bool rebootSystem;

        public Processor Processor { get; set; }
        public Memory Memory { get; set; }
        public Baseboard Baseboard { get; set; }
        public Storage Storage { get; set; }
        public OS OS { get; set; }

        public string? OrderId 
        { 
            get {  return orderId; }
            set
            {
                orderId = value;
                OnPropertyChanged("OrderId");
            }
        }
        public string? ProductId 
        { 
            get { return productId; }
            set
            {
                productId = value;
                OnPropertyChanged("ProductId");
            }
        }
        public string? ActivationKey 
        { 
            get { return activationKey; }
            set
            {
                activationKey = value;
                OnPropertyChanged("ActivationKey");
            }
        }

        public ObservableCollection<string> DebugList { get; set; } = new();
        public bool UseTestKey 
        { 
            get { return useTestKey; }
            set
            {
                useTestKey = value;
                OnPropertyChanged("UseTestKey");
            }
        }
        public bool Interactive
        {
            get { return interactive; }
            set
            {
                interactive = value;
                OnPropertyChanged("Interactive");
            }
        }
        public bool RebootSystem { 
            get { return rebootSystem; }
            set
            {
                rebootSystem = value;
                OnPropertyChanged("RebootSystem");
            }
        }

        public SystemInfo()
        {
            Memory = new Memory();
            OS = new OS();
            Processor = new();
            Baseboard = new Baseboard();
            Storage = new();
            ProductId = InjectionController.ParseXml();
            ActivationKey = InjectionController.CheckForInjection();

            SetAdvanced();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notify that the property changed.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// Set <see cref="OS.Edition"/> to <see cref="OS.Editions.HomeA"/> or <see cref="OS.Editions.ProA"/> if requirements are met.
        /// </summary>
        private void SetAdvanced()
        {
            if (OS.Edition == OS.Editions.Home &&
                    (Processor.Advanced || Memory.Advanced || Storage.Advanced))
            {
                OS.Edition = OS.Editions.HomeA;
            }
            else if (OS.Edition == OS.Editions.Pro && Processor.Advanced)
            {
                OS.Edition = Processor.Name switch
                {
                    string name when name.Contains("i9") => OS.Editions.ProA,
                    string name when name.Contains("Ryzen 9") => OS.Editions.ProA,
                    string name when name.Contains("Threadripper") => OS.Editions.ProA,
                    _ => OS.Editions.Pro,
                };
            }
        }
    }
}
