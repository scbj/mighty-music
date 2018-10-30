using Mighty_Music.Utils;
using Mighty_Music.Extensions;
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
        private FileInfo fileInfo;

        #region Properties
        public string Name { get { return Path.GetFileNameWithoutExtension(fileInfo.Name); } }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string CoverUrl { get; set; }
        #endregion

        public MusicFile(string path)
        {
            fileInfo = new FileInfo(path);
            ExtractMetadata();
        }
        public async Task Save()
        {
            Task<string> coverPath = DownloadAndSaveCoverAsync();

            using (var file = TagLib.File.Create(fileInfo.FullName))
            {
                file.Tag.Performers = new string[] { Artist };
                file.Tag.Title = Title;
                file.Tag.Album = Album;
                await coverPath;

                if (coverPath.Result != null)
                    file.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(coverPath.Result) };

                file.Save();
            }

            var newPath = fileInfo.DirectoryName + "\\" + this.ToString().Remove(Path.GetInvalidFileNameChars()) + fileInfo.Extension;
            if (fileInfo.FullName != newPath)
                fileInfo.MoveTo(newPath);
        }

        private void ExtractMetadata()
        {
            var fields = Filter.Flush(Name).Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries).Where(s => s.Trim() != "").ToArray();
            if (fields.Length >= 2)
            {
                Artist = fields[0];
                Title = fields[1];
                Album = fields[1];
            }
        }

        private async Task<string> DownloadAndSaveCoverAsync()
        {
            if (String.IsNullOrWhiteSpace(CoverUrl))
                return null;

            byte[] bytes = await new WebClient().DownloadDataTaskAsync(CoverUrl);

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

        public override string ToString() => $"{Artist} - {Title}";
    }
}
