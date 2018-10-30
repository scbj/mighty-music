using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using UWP.Extensions;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Models
{
    public class Music : IReadOnlyMusic
    {
        public BitmapImage Cover { get; private set; }

        public string Artsit { get; private set; }

        public string Title { get; private set; }

        public string Album { get; private set; }

        public async Task Load(StorageFile storageFile)
        {
            Stream fileStream = await WindowsRuntimeStorageExtensions.OpenStreamForReadAsync(storageFile);

            var file = TagLib.File.Create(new StreamFileAbstraction(storageFile.Name,
                fileStream, fileStream));

            await ReadTags(file);
        }

        private async Task ReadTags(TagLib.File tagFile)
        {
            Tag tags = tagFile.GetTag(TagTypes.Id3v2);
            Title = tags.Title;
            Artsit = tags.Performers.FirstOrDefault();
            Album = tags.Album;
            Cover = await tags.Pictures[0].ToBitmapImage(new Size(120, 120));
        }
    }
}
