using SBToolkit.Uwp.UI.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page, IView<SettingsViewModel>
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        public SettingsViewModel ViewModel => base.DataContext as SettingsViewModel;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


        }

        private async void MoveTreatedMusic_Toggled(object sender, RoutedEventArgs e)
        {
            SettingsViewModel settingsViewModel = (SettingsViewModel)DataContext;

            if (moveTreatedMusic.IsOn && settingsViewModel.DestinationFolderPath == null)
            {
                StorageFolder storageFolder = await settingsViewModel.PickFolder();
                if (storageFolder != null)
                    settingsViewModel.DestinationFolderPath = storageFolder.Path;
                else
                    moveTreatedMusic.IsOn = false;

            }

        }
    }
}
