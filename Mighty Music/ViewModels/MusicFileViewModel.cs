using Mighty_Music.Models;
using MVVM.Pattern.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_Music.ViewModels
{
    public class MusicFileViewModel : ViewModelBase
    {
        public MusicFile Model { get; private set; }

        public string Name
        {
            get { return Model.Name; }
        }
        public string Title
        {
            get { return Model.Title; }
            set
            {
                Model.Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Artist
        {
            get { return Model.Artist; }
            set
            {
                Model.Artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }
        public string Album
        {
            get { return Model.Album; }
            set
            {
                Model.Album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        public Cover Cover
        {
            get { return null; }
            set
            {
                Model.CoverUrl = value.Url;
            }
        }

        public MusicFileViewModel(MusicFile model)
        {
            Model = model;
        }

        public override string ToString() => Model.ToString();
    }
}
