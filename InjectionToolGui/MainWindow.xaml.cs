using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace InjectionToolGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SystemInfo SystemInfo = new();

        public MainWindow()
        {
            InitializeComponent();
            CheckSettings();

            this.DataContext = SystemInfo;

            DebugList.ItemsSource = SystemInfo.DebugList;

            try
            {
                InjectionController.Initialization();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            InitializeRadios();

            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Finished Initialization");
        }
        /// <summary>
        /// Verify that secure boot, tpm, and core isolation are enabled and functioning. Check if injection
        /// has already been done.
        /// </summary>
        /// <see cref="SystemSettings"/>
        /// <seealso cref="SystemInfo.ActivationKey"/>
        private void CheckSettings()
        {
            try
            {
                SystemSettings.CheckAllSettings();

                if (!SystemSettings.Network) { throw new Exception("No network connection"); }

                InjectButton.IsEnabled = SystemSettings.SecureBoot && SystemSettings.TPM && SystemSettings.CoreIsolation;
                if (!SystemSettings.SecureBoot || !SystemSettings.TPM || !SystemSettings.CoreIsolation)
                {
                    MessageBox.Show("Secure Boot, TPM, and Core Isolation must be enabled before injection.\n" +
                        $"Secure Boot : {SystemSettings.SecureBoot}\n" +
                        $"TPM: {SystemSettings.TPM}\n" +
                        $"Core Isolation: {SystemSettings.CoreIsolation}\n",
                            "Attention!", MessageBoxButton.OK, MessageBoxImage.Error);

                    InjectButton.IsEnabled = false;
                    OrderIdTextBox.IsEnabled = false;

                    SystemInfo.OrderId = "Required Settings Disabled";
                }

                if (SystemInfo.ActivationKey != null)
                {
                    InjectButton.IsEnabled = false;
                    OrderIdTextBox.IsEnabled = false;

                    SystemInfo.OrderId = "Key Present";
                }
            }
            catch (Exception ex)
            {
                SystemInfo.DebugList.Insert(0, ex.Message);
            }
        }

        /// <summary>
        /// Pre-select radios based on hardware and software detected in the system.
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="SystemInfo.Baseboard"/>
        private void InitializeRadios()
        {
            // Manufacturer
            switch (SystemInfo.Baseboard.Manufacturer)
            {
                case Baseboard.MANUFACTURER.ASROCK:
                    MsiAsrockRadio.IsChecked = true;
                    break;
                case Baseboard.MANUFACTURER.ASUS:
                    AsusRadio.IsChecked = true;
                    break;
                case Baseboard.MANUFACTURER.ASUSZ690:
                    AsusZ690Radio.IsChecked = true;
                    break;
                case Baseboard.MANUFACTURER.MSI:
                    MsiAsrockRadio.IsChecked = true;
                    break;
                case Baseboard.MANUFACTURER.SAGER:
                    SagerRadio.IsChecked = true;
                    break;
                case Baseboard.MANUFACTURER.SAGERH2O:
                    SagerH2oRadio.IsChecked = true;
                    break;
                default:
                    GenericRadio.IsChecked = true;
                    break;
            }

            // Version
            switch (SystemInfo.OS.Version)
            {
                case "11":
                    Win11Radio.IsChecked = true;
                    break;
                case "10":
                    Win10Radio.IsChecked = true;
                    break;
                default:
                    break;
            }

            // Edition
            switch (SystemInfo.OS.Edition)
            {
                case OS.Editions.Home:
                    HomeRadio.IsChecked = true;
                    break;
                case OS.Editions.HomeA:
                    HomeAdvancedRadio.IsChecked = true;
                    break;
                case OS.Editions.Pro:
                    ProRadio.IsChecked = true;
                    break;
                case OS.Editions.ProA:
                    ProAdvancedRadio.IsChecked = true;
                    break;
                case OS.Editions.RDPK:
                    RdpkRadio.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Control visibility for the progress bar.
        /// </summary>
        private void ProgressBarControl()
        {
            if (Progress.IsVisible)
            {
                Progress.Visibility = Visibility.Hidden;
            }
            else
            {
                Progress.Visibility = Visibility.Visible;
            }
        }

        /*
         * MENU ITEMS
         */

        /// <summary>
        /// Closes the application.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Uses InjectionController to inject a previously pulled key.
        /// </summary>
        /// <see cref="InjectionController.InjectOldKey(bool, SystemInfo)"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuInject_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarControl();
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Starting injection from menu");
            await InjectionController.InjectOldKey(DebugGroupBox.IsVisible, SystemInfo);
            ProgressBarControl();

            if (SystemInfo.RebootSystem)
            {
                Process.Start("cmd.exe", "shutdown -r -t 0");
            }
        }

        /// <summary>
        /// Uses InjectionController to report the current key.
        /// </summary>
        /// <see cref="InjectionController.ReportKey(bool, string)"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuReport_Click(object sender, RoutedEventArgs e)
        {
            if (SystemInfo.OrderId != null && SystemInfo.OrderId != "")
            {
                ProgressBarControl();
                SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Starting report");
                await InjectionController.ReportKey(DebugGroupBox.IsVisible, SystemInfo.OrderId);
                ProgressBarControl();
            }
        }

        /// <summary>
        /// Uses InjectionController to return a non-reported key.
        /// </summary>
        /// <see cref="InjectionController.ReturnKey(bool, string)"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuReturn_Click(object sender, RoutedEventArgs e)
        {
            if (SystemInfo.OrderId != null && SystemInfo.OrderId != "")
            {
                ProgressBarControl();
                SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Starting return");
                await InjectionController.ReturnKey(DebugGroupBox.IsVisible, SystemInfo.OrderId);
                ProgressBarControl();
            }
        }

        /// <summary>
        /// Uses InjectionController to clear an OA3 key.
        /// </summary>
        /// <see cref="InjectionController.ClearKey(SystemInfo)"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuClear_Click(object sender, RoutedEventArgs e)
        {
            ProgressBarControl();
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Starting clear");
            await InjectionController.ClearKey(SystemInfo);
            ProgressBarControl();
        }

        /// <summary>
        /// Uses InjectionController to upload log files to dropbox
        /// </summary>
        /// <see cref="InjectionController.UploadLogs(bool, string)"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuUploadLogs_Click(object sender, RoutedEventArgs e)
        {
            if (SystemInfo.OrderId != null && SystemInfo.OrderId != "")
            {
                ProgressBarControl();
                SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Starting upload");
                await InjectionController.UploadLogs(DebugGroupBox.IsVisible, SystemInfo.OrderId);
                ProgressBarControl();
            }
        }

        /// <summary>
        /// Enables debug menu in MainWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem)
            {
                DebugGroupBox.Visibility = menuItem.IsChecked ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /*
         * TEXT BOX CHANGES
         */

        /// <summary>
        /// Sets SystemInfo.OrderId
        /// </summary>
        /// <see cref="SystemInfo.OrderId"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OrderIdTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SystemInfo.OrderId = OrderIdTextBox.Text;

            InjectButton.IsEnabled = SystemInfo.OrderId != "";
        }

        /// <summary>
        /// Sets SystemInfo.Baseboard.Name
        /// </summary>
        /// <see cref="SystemInfo.Baseboard"/>
        /// <seealso cref="Baseboard.Name"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotherboardTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SystemInfo.Baseboard.Name = MotherboardTextBox.Text;
        }

        /*
        * Manufacturer Radios
        */
        /// <summary>
        /// Sets SystemInfo.Baseboard.Manufacturer Asrock
        /// </summary>
        /// <see cref="SystemInfo.Baseboard"/>
        /// <seealso cref="Baseboard.Manufacturer"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenericRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.Baseboard.Manufacturer = Baseboard.MANUFACTURER.ASROCK;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Manufacturer set to {SystemInfo.Baseboard.Manufacturer}");
        }

        /// <summary>
        /// Sets SystemInfo.Baseboard.Manufacturer Asus
        /// </summary>
        /// <see cref="SystemInfo.Baseboard"/>
        /// <seealso cref="Baseboard.Manufacturer"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AsusRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.Baseboard.Manufacturer = Baseboard.MANUFACTURER.ASUS;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Manufacturer set to {SystemInfo.Baseboard.Manufacturer}");
        }

        /// <summary>
        /// Sets SystemInfo.Baseboard.Manufacturer Sager
        /// </summary>
        /// <see cref="SystemInfo.Baseboard"/>
        /// <seealso cref="Baseboard.Manufacturer"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SagerRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.Baseboard.Manufacturer = Baseboard.MANUFACTURER.SAGER;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Manufacturer set to {SystemInfo.Baseboard.Manufacturer}");
        }

        /// <summary>
        /// Sets SystemInfo.Baseboard.Manufacturer SagerH2O
        /// </summary>
        /// <see cref="SystemInfo.Baseboard"/>
        /// <seealso cref="Baseboard.Manufacturer"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SagerH2ORadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.Baseboard.Manufacturer = Baseboard.MANUFACTURER.SAGERH2O;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Manufacturer set to {SystemInfo.Baseboard.Manufacturer}");
        }

        /// <summary>
        /// Sets SystemInfo.Baseboard.Manufacturer MSI
        /// </summary>
        /// <see cref="SystemInfo.Baseboard"/>
        /// <seealso cref="Baseboard.Manufacturer"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsiAsrockRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.Baseboard.Manufacturer = Baseboard.MANUFACTURER.MSI;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Manufacturer set to {SystemInfo.Baseboard.Manufacturer}");
        }

        /// <summary>
        /// Sets SystemInfo.Baseboard.Manufacturer AsusZ690
        /// </summary>
        /// <see cref="SystemInfo.Baseboard"/>
        /// <seealso cref="Baseboard.Manufacturer"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AsusZ690Radio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.Baseboard.Manufacturer = Baseboard.MANUFACTURER.ASUSZ690;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Manufacturer set to {SystemInfo.Baseboard.Manufacturer}");
        }

        /*
         * Version Radios 
         */

        /// <summary>
        /// Sets SystemInfo.OS.Version to 10
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="OS.Version"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win10Radio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.OS.Version = "Windows 10";
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Version set to {SystemInfo.OS.Version}");
        }

        /// <summary>
        /// Sets SystemInfo.OS.Version to 11
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="OS.Version"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Win11Radio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.OS.Version = "Windows 11";
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Version set to {SystemInfo.OS.Version}");
        }

        /*
         * Edition Radios
        */

        /// <summary>
        /// Sets SystemInfo.OS.Edition to Home
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="OS.Edition"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.OS.Edition = OS.Editions.Home;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Edition set to {SystemInfo.OS.Edition}");
        }

        /// <summary>
        /// Sets SystemInfo.OS.Edition to Pro
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="OS.Edition"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.OS.Edition = OS.Editions.Pro;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Edition set to {SystemInfo.OS.Edition}");

        }

        /// <summary>
        /// Sets SystemInfo.OS.Edition to RDPK
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="OS.Edition"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RdpkRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.OS.Edition = OS.Editions.RDPK;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Edition set to {SystemInfo.OS.Edition}");
        }

        /// <summary>
        /// Sets SystemInfo.OS.Edition to Home Advanced
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="OS.Edition"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeAdvancedRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.OS.Edition = OS.Editions.HomeA;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Edition set to {SystemInfo.OS.Edition}");
        }

        /// <summary>
        /// Sets SystemInfo.OS.Edition to Pro Advanced
        /// </summary>
        /// <see cref="SystemInfo.OS"/>
        /// <seealso cref="OS.Edition"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProAdvancedRadio_Click(object sender, RoutedEventArgs e)
        {
            SystemInfo.OS.Edition = OS.Editions.ProA;
            SystemInfo.DebugList.Insert(0, $"{DateTime.Now} Edition set to {SystemInfo.OS.Edition}");
        }

        /*
         * Buttons
         */
        /// <summary>
        /// Uses InjectionController to pull a new key from the server and inject it.
        /// </summary>
        /// <see cref="InjectionController.InjectNewKey(bool, SystemInfo)"/>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void InjectButton_Click(object sender, RoutedEventArgs e)
        {
            InjectButton.IsEnabled = false;

            try
            {
                ProgressBarControl();
                if (!SystemSettings.CheckNetwork()) { throw new Exception("Network Connection Required. Please connect and try again."); }
                await InjectionController.InjectNewKey(DebugGroupBox.IsVisible, SystemInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error While Injecting", MessageBoxButton.OK, MessageBoxImage.Error);
                if (!InjectionController.IsInjected) { InjectButton.IsEnabled = true; }
            }

            SystemInfo.ProductId = InjectionController.ParseXml();
            SystemInfo.ActivationKey = InjectionController.CheckForInjection();
            ProgressBarControl();
        }
    }
}
