using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Models
{
    public interface IMusic : IReadOnlyMusic
    {
        new string Artsit { get; set; }

        new string Title { get; set; }

        new string Album { get; set; }

        new BitmapImage Cover { get; set; }
    }
}