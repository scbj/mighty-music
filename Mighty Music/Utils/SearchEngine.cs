using Mighty_Music.Extensions;
using Mighty_Music.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
#if DEBUG
            browser.Dock = DockStyle.Fill;
            var form = new Form();
            form.Controls.Add(browser);
            form.Show();
#endif
        }

        public static async void SearchCovers(string query)
        {
            while(!await HasInternet())
                await Task.Delay(3000);

            SearchEngine.query = query;
            browser.Navigate(GOOGLE_IMAGE_URL);
        }

        private async static void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            void WriteQuery()
            {
                // Get the search input
                HtmlElement input = browser.Document
                    .GetElementsByTagName("input")
                    .ToList()
                    .Single(el => el.GetAttribute("className") == "gLFyf gsfi");

                // Fill it with the query
                input.InnerText = query;
            }

            void ClickSearchButton()
            {
                // Get the search button
                HtmlElement button = browser.Document
                    .GetElementsByTagName("button")
                    .ToList()
                    .Single(el => el.GetAttribute("className") == "Tg7LZd");

                // Simulate a click event on the button
                button.InvokeMember("click");
            }

            void ListCover ()
            {
                var covers = new List<Cover>();
                var divs = browser.Document.All.ToList().Where(div => div.GetAttribute("className") == "rg_bx rg_di rg_el ivg-i").ToList();
                for (int i = 0; i < divs.Count; i++)
                {
                    HtmlElement div = divs[i].All.ToList().Single(d => d.GetAttribute("className") == "rg_ilmbg" && d.InnerText.Contains('×'));
                    string[] sizes = div.InnerText.Split('-')[0].Trim().Split('×');
                    int x = Int32.Parse(sizes[0].Trim()),
                        y = Int32.Parse(sizes[1].Trim());

                    if (x == y && x >= 500)
                    {
                        HtmlElement a = divs[i]
                            .GetElementsByTagName("a")
                            .ToList()
                            .SingleOrDefault(el => !el.GetAttribute("href").StartsWith("https://www.google.com/search?tbm"));

                        if (a == null)
                            continue;

                        string href = a.GetAttribute("href");

                        covers.Add(new Cover(i + 1, href.Replace(/*"https://www.google.com*/"/imgres?imgurl=", "").Split('&')[0].DecodeUrl()));
                    }
                    if (covers.Count > 10 || (i > 20 && covers.Count >= 5))
                        break;
                }

                SearchCompleted?.Invoke(covers);
            }

            try
            {
                if (e.Url.AbsoluteUri.Contains(GOOGLE_IMAGE_URL))
                {
                    WriteQuery();

                    await Task.Delay(200);

                    ClickSearchButton();

                    await Task.Delay(200);
                }
                else if (e.Url.AbsoluteUri.Contains(GOOGLE_SEARCH_RESULT))
                {
                    await Task.Delay(500);

                    ListCover();
                }
            }
            catch(Exception ex)
            {
                string message = "Une erreur est survenue, veuillez contacter l'éditeur avec une capture d'écran de ce message ainsi que le fichier qui a provoqué l'erreur.\n\n" +
                    ex.Message + "\n" +
                    ex.ToString();

                MessageBox.Show(message, "Erreur " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static async Task<bool> HasInternet()
        {
            var ping = new Ping();
            try
            {
                PingReply reply = await ping.SendPingAsync("8.8.8.8", 1000);

                return ping.Send("8.8.8.8", 1000).Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
