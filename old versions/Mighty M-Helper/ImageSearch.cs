using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace Mighty_M_Helper
{
    public static class ImageSearch
    {
        public static async Task<IEnumerable<Cover>> GetCovers(string artist, string title)
        {
            artist = artist.Replace(' ', '+').Replace("&", "%26");
            title = title.Replace(' ', '+').Replace("&", "%26");

            var browser = new WebBrowser();
            await browser.NavigateAsync($"https://www.google.fr/search?q={artist}+{title}+cover&tbm=isch");

            // Select each image from pages
            var divPictures = (from div in browser.Document.GetElementsByTagName("div").ToList()
                               where div.GetAttribute("className") == "rg_di rg_el ivg-i"
                               select div).ToList();

            var covers = new List<Cover>();

            for (int i = 0; i < divPictures.Count; i++)
            {
                var size = divPictures[i].GetElementsByTagName("span").ToList().Single(e => e.GetAttribute("className") == "rg_ilmn");
                var sizes = size.InnerText.Split('-')[0].Split('×');
                int x = int.Parse(sizes[0].Trim()),
                    y = int.Parse(sizes[1].Trim());
                if (x == y && x >= 300)
                    covers.Add(new Cover() { Rank = i+1, Url = ImageSearch.ParseUrl(divPictures[i].GetElementsByTagName("a")[0].GetAttribute("href")) });
                if (covers.Count > 10 || i > 20)
                    break;
            }

            return covers;
        }

        public static string ParseUrl(string source)
        {
            return source.Substring(source.IndexOf("imgurl=") + 7, source.LastIndexOf("imgrefurl=") - 39);
        }
    }
}
