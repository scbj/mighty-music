using MVVM.Pattern__UWP_.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UWP.Controls;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace UWP.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        #region Properties
        public string SourceFolderPath
        {
            get { return ApplicationSettings.SourceFolderPath; }
            set
            {
                ApplicationSettings.SourceFolderPath = value;
                OnPropertyChanged(nameof(SourceFolderPath));
            }
        }
        public string DestinationFolderPath
        {
            get { return ApplicationSettings.DestinationFolderPath; }
            set
            {
                ApplicationSettings.DestinationFolderPath = value;
                OnPropertyChanged(nameof(DestinationFolderPath));
            }
        }
        public bool IsMoveFileEnable
        {
            get { return ApplicationSettings.IsMoveFileEnable; }
            set
            {
                if (ApplicationSettings.IsMoveFileEnable)
                    DestinationFolderPath = null;

                OnPropertyChanged(nameof(IsMoveFileEnable));
            }
        }
        public List<string> Filters { get { return ApplicationSettings.Filters.ToList(); } }
        public bool IsSaveCreationTimeEnable
        {
            get { return ApplicationSettings.IsSaveCreationTimeEnable; }
            set
            {
                ApplicationSettings.IsSaveCreationTimeEnable = value;
                OnPropertyChanged(nameof(IsSaveCreationTimeEnable));
            }
        }
        public bool IsDateTreatingEnable
        {
            get { return ApplicationSettings.IsDateTreatingEnable; }
            set
            {
                ApplicationSettings.IsDateTreatingEnable = value;
                OnPropertyChanged(nameof(IsDateTreatingEnable));
            }
        }

        public ICommand BrowseSourceFolderCommand { get; }
        public ICommand BrowseDestFolderCommand { get; }
        public ICommand AddFilterCommand { get; }
        public ICommand RemoveFilterCommand { get; }
        #endregion

        public SettingsViewModel()
        {
            ApplicationSettings.Initialize();

            // UI Commands
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
                            DestinationFolderPath = t.Result.Path;
                    });
                });
            });
            AddFilterCommand = new RelayCommand(AddFilter);
            RemoveFilterCommand = new RelayCommand(RemoveFilter, (param) => param != null);
        }

        private async void AddFilter(object param)
        {
            var inputContentDialog = new InputContentDialog("NOUVEAU FILTRE")
            {
                PrimaryButtonText = "Ajouter",
                SecondaryButtonText = "Annuler"
            };
            ContentDialogResult result = await inputContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                if (String.IsNullOrWhiteSpace(inputContentDialog.Input))
                {
                    var messageDialog = new MessageDialog("Un filtre ne doit pas être vide ou contenir que des espaces", "Invalide");
                    await messageDialog.ShowAsync();
                }
                else
                {
                    if (ApplicationSettings.Filters.Contains(inputContentDialog.Input))
                    {
                        var dialog = new MessageDialog("Cet élément est déjà présent ! Il ne sera pas dupliqué.", "Élément existant");
                        await dialog.ShowAsync();
                    }
                    else
                    {
                        ApplicationSettings.Filters.Add(inputContentDialog.Input);
                        OnPropertyChanged(nameof(Filters));
                    }
                }
            }


        }

        private void RemoveFilter(object param)
        {
            var filter = (string)param;
            if (Filters.Contains(filter))
            {
                ApplicationSettings.Filters.Remove(filter);
                OnPropertyChanged(nameof(Filters));
            }
        }

        public async Task<StorageFolder> PickFolder()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            folderPicker.FileTypeFilter.Add(".mp3");
            return await folderPicker.PickSingleFolderAsync();
        }
    }
}
