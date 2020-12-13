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
                file.RemoveTags(TagLib.TagTypes.Id3v1);
                file.RemoveTags(TagLib.TagTypes.Id3v2);
                TagLib.Tag tag = file.GetTag(TagLib.TagTypes.Id3v2, true);

                tag.Performers = Artist.Split(',');
                tag.Title = Title;
                tag.Album = Album;
                await coverPath;

                if (coverPath.Result != null)
                    tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(coverPath.Result) };

                file.Save();
                UpdateTimeAtOldestDate(fileInfo.FullName);
            }

            string newPath = fileInfo.DirectoryName + "\\" + ToString() + fileInfo.Extension;
            if (fileInfo.FullName != newPath)
                fileInfo.MoveTo(newPath);
        }

        private void UpdateTimeAtOldestDate(string path)
        {
            var dates = new List<DateTime>
            {
                File.GetCreationTime(path),
                File.GetLastAccessTime(path),
                File.GetLastWriteTime(path)
            };

            // Get the oldest time from all
            DateTime oldest = dates.Min();

            // Update all the different time of the file to the oldest
            File.SetCreationTime(path, oldest);
            File.SetLastAccessTime(path, oldest);
            File.SetLastWriteTime(path, oldest);
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

        public override string ToString() => $"{Artist} - {Title}".Remove(Path.GetInvalidFileNameChars());
    }
}
