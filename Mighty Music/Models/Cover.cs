using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_Music.Models
{
    public class Cover
    {
        public int Rank { get; set; }
        public string Url { get; set; }

        public Cover(int rank, string url)
        {
            Rank = rank;
            Url = url;
        }
    }
}
