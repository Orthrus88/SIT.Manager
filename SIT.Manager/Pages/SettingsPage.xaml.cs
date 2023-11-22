using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SIT.Manager.Classes;
using System;
using System.Reflection;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SIT.Manager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            DataContext = App.ManagerConfig;

            VersionHyperlinkButton.Content = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }

        private async void ChangeInstallButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new()
            {
                SuggestedStartLocation = PickerLocationId.ComputerFolder
            };

            folderPicker.FileTypeFilter.Add("*");

            Window window = new();
            IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            StorageFolder eftFolder = await folderPicker.PickSingleFolderAsync();
            if (eftFolder != null)
            {
                App.ManagerConfig.InstallPath = eftFolder.Path;

                Utils.CheckEFTVersion(eftFolder.Path);
                Utils.CheckSITVersion(eftFolder.Path);

                App.ManagerConfig.Save();

                Utils.ShowInfoBar("Settings:", $"Install path set to {eftFolder.Path}");
            }
        }

        private void VersionHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new()
            {
                RequestedOperation = DataPackageOperation.Copy,
            };

            dataPackage.SetText(VersionHyperlinkButton.Content.ToString());            
            Clipboard.SetContent(dataPackage);
        }
    }
}
