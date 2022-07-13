using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;

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
        private string? version;

        public Processor Processor { get; set; }
        public Memory Memory { get; set; }
        public Baseboard Baseboard { get; set; }
        public Storage Storage { get; set; }
        public OS OS { get; set; }

        public string? OrderId
        {
            get { return orderId; }
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
        public string? Version
        {
            get { return version; }
            private set
            {
                version = value;
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
        public bool RebootSystem
        {
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

            Version? version = Assembly.GetExecutingAssembly().GetName().Version;
            Version = version != null ? version.ToString() : "Not Found";

            SetAdvanced();
            CheckForQcLog();
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

        /// <summary>
        /// Pull the order id from the qc log if it exists.
        /// </summary>
        private void CheckForQcLog()
        {
            if (File.Exists(@"C:\Recovery\QC\QC.json"))
            {
                try
                {
                    using FileStream fileStream = new(@"C:\Recovery\QC\QC.json", FileMode.Open, FileAccess.Read);
                    using StreamReader streamReader = new(fileStream);
                    string Json = streamReader.ReadToEnd();
                    JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(Json);

                    OrderId = jsonElement.GetProperty("ID").ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"C:\Recovery\QC\QC.json");
                }
            }
        }
    }
}
