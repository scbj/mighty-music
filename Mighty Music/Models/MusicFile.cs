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
    public class MusicFile
    {
        private string path;

        public string Name { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string CoverUrl { get; set; }

        public MusicFile(string path)
        {
            this.path = path;
        }

        public void Initialize()
        {
            Name = Path.GetFileNameWithoutExtension(path);
            var option = StringSplitOptions.RemoveEmptyEntries;
            var fields = (from s in Filter.Flush(Name).Split(new string[] { " - " }, option)
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
            file.Tag.Performers = new string[] { Artist };
            file.Tag.Title = Title;
            file.Tag.Album = Album;

            await coverPath;
            var pictures = new TagLib.IPicture[] { new TagLib.Picture(coverPath.Result) };
            file.Tag.Pictures = pictures;
            file.Save();
            
            //File.Move(path, Path.GetDirectoryName(path) + $"\\{Artist} - {Title}.mp3");
        }

        public async Task<string> GetCoverAsync()
        {
            var bytes = await new WebClient().DownloadDataTaskAsync(CoverUrl);

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
    }
}
