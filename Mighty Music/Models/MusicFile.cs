using Mighty_Music.Utils;
using MVVM.Pattern.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Mighty_Music.Models
{
    public class MusicFile : ViewModelBase
    {
        private string path;
        private string name;
        private string title;
        private string artist;
        private string album;
        private string coverUrl;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        public string Artist
        {
            get { return artist; }
            set
            {
                artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }
        public string Album
        {
            get { return album; }
            set
            {
                album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        public Cover Cover
        {
            get { return null; }
            set
            {
                coverUrl = value.Url;
            }
        }

        public MusicFile(string path)
        {
            this.path = path;
        }

        public void Initialize()
        {
            Name = Path.GetFileNameWithoutExtension(path);
            var option = StringSplitOptions.RemoveEmptyEntries;
            var fields = (from s in Filter.Flush(name).Split(new string[] { " - " }, option)
                          where s.Trim() != ""
                          select s).ToArray();

            if (fields.Length >= 2)
            {
                Artist = fields[0];
                Title = fields[1];
                Album = fields[1];
            }
        }
        public async Task Save()
        {

            Task<string> coverPath = GetCoverAsync();

            var file = TagLib.File.Create(path);
            file.Tag.Performers = new string[] { artist };
            file.Tag.Title = title;
            file.Tag.Album = album;

            await coverPath;
            var pictures = new TagLib.IPicture[] { new TagLib.Picture(coverPath.Result) };
            file.Tag.Pictures = pictures;
            file.Save();
        }

        public async Task<string> GetCoverAsync()
        {
            var bytes = await new WebClient().DownloadDataTaskAsync(coverUrl);

            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(bytes); ;
            bi.DecodePixelHeight = 500; bi.DecodePixelWidth = 500;
            bi.EndInit();
            bi.Freeze();

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bi));

            if (!Directory.Exists("covers"))
                Directory.CreateDirectory("covers");
            string pathToSave = "covers\\" + ToString() + ".jpg";

            using (var filestream = new FileStream(pathToSave, FileMode.Create))
                encoder.Save(filestream);

            return pathToSave;
        }

        public override string ToString() => $"{artist} - {title}";
    }
}
