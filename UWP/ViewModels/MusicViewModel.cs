using SBToolkit.Uwp.UI.Input;
using SBToolkit.Uwp.UI.Observable;
using SBToolkit.Uwp.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UWP.Models;
using UWP.Web.Google;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace UWP.ViewModels
{
    public class MusicViewModel : ObservableObject, IViewModel
    {
        private ObservableCollection<IReadOnlyMusic> _musics;

        public MusicViewModel()
        {
            _musics = new ObservableCollection<IReadOnlyMusic>();

            BrowseCommand = new RelayCommand(Browse);
        }

        public ObservableCollection<IReadOnlyMusic> Musics
        {
            get => _musics;
            set => Set(ref _musics, value);
        }

        public ICommand BrowseCommand { get; }

        private async void Browse()
        {
            var openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            openPicker.FileTypeFilter.Add(".mp3");

            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            
            foreach (StorageFile file in files)
            {
                try
                {
                    var music = new Music();
                    await music.Load(file);

                    Musics.Add(music);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            if (Musics.Count > 0)
            {
                var google = new GoogleImages();

                foreach (IReadOnlyMusic music in Musics)
                {
                    List<string> urls = await google.SearchImages(music.Title + " soundcloud");
                }                
            }
        }
    }
}
