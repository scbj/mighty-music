using MVVM.Pattern.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mighty_Music.Models
{
    public class Cover
    {
        public int Rank { get; set; }
        public string Url { get; set; }
        public ICommand OpenCommand { get; set; }

        public Cover(int rank, string url)
        {
            Rank = rank;
            Url = url;
            OpenCommand = new RelayCommand((args) => Process.Start((string)args));
        }
    }
}
