using Mighty_Music.Extensions;
using Mighty_Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mighty_Music.Utils
{
    public static class SearchEngine
    {
        private static WebBrowser browser = new WebBrowser();

        public static async Task<List<Cover>> GetCovers(string query, string keyword)
        {
            var covers = new List<Cover>();

            await browser.NavigateAsync("https://images.google.com/");
            browser.Document.GetElementById("lst-ib").SetAttribute("value", query + " " + keyword);
            await Task.Delay(200);
            var btn = browser.Document.All.GetElementsByName("btnG").ToList().Single().InvokeMember("click");
            await browser.Wait();

            var divs = browser.Document.GetElementById("rg_s").Children.ToList().Where(he => he.GetAttribute("className") == "rg_di rg_el ivg-i").ToList();

            for(int i = 0; i < divs.Count; i++)
            {
                var span = divs[i].GetElementsByTagName("span").ToList().Single();
                var sizes = span.InnerText.Split('-')[0].Trim().Split('×');
                int x = int.Parse(sizes[0].Trim()),
                    y = int.Parse(sizes[1].Trim());

                if (x == y && x >= 300)
                {
                    var href = divs[i].GetElementsByTagName("a").ToList().Single().GetAttribute("href");
                    covers.Add(new Cover(i + 1, href.Replace("https://www.google.com/imgres?imgurl=", "").Split('&')[0].DecodeUrl()));
                }
                if (covers.Count > 10 || i > 20)
                    break;
            }

            return covers;
        }
    }
}
