using MVVM.Pattern__UWP_.ViewModel;
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
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
        }

        private async void moveTreatedMusic_Toggled(object sender, RoutedEventArgs e)
        {
            SettingsViewModel settingsViewModel = (SettingsViewModel)DataContext;

            if (moveTreatedMusic.IsOn && settingsViewModel.DestFolderPath == null)
            {
                StorageFolder storageFolder = await settingsViewModel.PickFolder();
                if (storageFolder != null)
                    settingsViewModel.DestFolderPath = storageFolder.Path;
                else
                    moveTreatedMusic.IsOn = false;

            }

        }
    }
}
