using MVVM.Pattern__UWP_.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;

namespace UWP.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        #region Fields
        ApplicationDataContainer localSettings;
        #endregion

        #region Properties
        public string SourceFolderPath
        {
            get { return (string)localSettings.Values[nameof(SourceFolderPath)]; }
            set
            {
                localSettings.Values[nameof(SourceFolderPath)] = value;
                OnPropertyChanged(nameof(SourceFolderPath));
            }
        }
        public string DestFolderPath
        {
            get { return (string)localSettings.Values[nameof(DestFolderPath)]; ; }
            set
            {
                localSettings.Values[nameof(DestFolderPath)] = value;
                OnPropertyChanged(nameof(DestFolderPath));
            }
        }
        public bool IsMoveFileEnable
        {
            get { return DestFolderPath != null; }
            set
            {
                if (IsMoveFileEnable)
                    DestFolderPath = null;

                OnPropertyChanged(nameof(IsMoveFileEnable));
            }
        }

        public ICommand BrowseSourceFolderCommand { get; }
        public ICommand BrowseDestFolderCommand { get; }
        #endregion

        public SettingsViewModel()
        {
            localSettings = ApplicationData.Current.LocalSettings;
            BrowseSourceFolderCommand = new RelayCommand((s) =>
            {
                PickFolder().ContinueWith(async (t) =>
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (t.Result != null)
                            SourceFolderPath = t.Result.Path;
                    });
                });
            });
            BrowseDestFolderCommand = new RelayCommand((s) =>
            {
                PickFolder().ContinueWith(async (t) =>
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (t.Result != null)
                            DestFolderPath = t.Result.Path;
                    });
                });
            });
        }

        public async Task<StorageFolder> PickFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary;
            folderPicker.FileTypeFilter.Add(".mp3");
            return await folderPicker.PickSingleFolderAsync();
        }
    }
}
