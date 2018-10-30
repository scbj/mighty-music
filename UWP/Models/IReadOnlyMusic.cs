using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Models
{
    public interface IReadOnlyMusic
    {
        string Artsit { get; }

        string Title { get; }

        string Album { get; }

        BitmapImage Cover { get; }
    }
}
