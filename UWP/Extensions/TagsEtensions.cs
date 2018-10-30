using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TagLib;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Extensions
{
    public static class TagsEtensions
    {
        public static async Task<BitmapImage> ToBitmapImage(this IPicture picture, Size size)
        {
            var bitmap = new BitmapImage();

            using (var stream = new InMemoryRandomAccessStream())
            {
                await stream.WriteAsync(picture.Data.Data.AsBuffer());
                stream.Seek(0);

                int origHeight = bitmap.PixelHeight;
                int origWidth = bitmap.PixelWidth;
                float ratioX = (float)size.Width / origWidth;
                float ratioY = (float)size.Height / origHeight;
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
