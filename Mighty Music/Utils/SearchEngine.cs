﻿using Mighty_Music.Models;
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
        // Fields
        private static WebBrowser browser;
        private static string query;

        const string GOOGLE_IMAGE_URL = "https://images.google.com";
        const string GOOGLE_SEARCH_RESULT = "https://www.google.com/search?";


        // Events
        public static event Action<List<Cover>> SearchCompleted;


        public static void Initialize()
        {
            browser = new WebBrowser();
            browser.DocumentCompleted += DocumentCompleted;

            //browser.Dock = DockStyle.Fill;
            //var form = new Form();
            //form.Controls.Add(browser);
            //form.Show();
        }

        public static void SearchCovers(string query)
        {
            SearchEngine.query = query;
            browser.Navigate(GOOGLE_IMAGE_URL);
        }

        private async static void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsoluteUri.Contains(GOOGLE_IMAGE_URL))
            {
                browser.Document.GetElementById("lst-ib").InnerText = query;
                await Task.Delay(200);
                browser.Document.All.GetElementsByName("btnG").ToList().Single().InvokeMember("click");
            }
            else if (e.Url.AbsoluteUri.Contains(GOOGLE_SEARCH_RESULT))
            {
                var covers = new List<Cover>();

                var divs = browser.Document.GetElementById("rg_s").Children.ToList().Where(he => he.GetAttribute("className") == "rg_di rg_el ivg-i").ToList();
                for (int i = 0; i < divs.Count; i++)
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

                SearchCompleted?.Invoke(covers);
            }
        }
    }

    public static class Extensions
    {
        public static List<HtmlElement> ToList(this HtmlElementCollection collection) => collection.Cast<HtmlElement>().ToList();

        public static string DecodeUrl(this string encoded)
        {
            return encoded.Replace("%253F", "?")
                .Replace("%253D", "=")
                .Replace("%2525", "%");
        }
    }
}
