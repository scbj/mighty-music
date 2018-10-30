using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.TagLib.Extensions
{
    public static class TagsEtensions
    {
        public static async Task<BitmapImage> ToBitmapImage(this IPicture picture)
        {
            var bitmap = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(picture.Data.Data.AsBuffer());
                stream.Seek(0);

                int origHeight = bitmap.PixelHeight;
                int origWidth = bitmap.PixelWidth;
                float ratioX = 120 / (float)origWidth;
                float ratioY = 120 / (float)origHeight;
                float ratio = Math.Min(ratioX, ratioY);
                int newHeight = (int)(origHeight * ratio);
                int newWidth = (int)(origWidth * ratio);

                bitmap.DecodePixelHeight = newHeight;
                bitmap.DecodePixelWidth = newWidth;

                await bitmap.SetSourceAsync(stream);
            }

            return bitmap;
        }
    }
}
