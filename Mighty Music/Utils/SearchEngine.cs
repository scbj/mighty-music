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

        private static void WriteQuery ()
        {
            // Get the search input
            HtmlElement input = browser.Document
                .GetElementsByTagName("input")
                .ToList()
                .Single(el => el.GetAttribute("className") == "gLFyf gsfi");

            // Fill it with the query
            input.InnerText = query;
        }

        private static void ClickSearchButton()
        {
            // Get the search button
            HtmlElement button = browser.Document
                .GetElementsByTagName("button")
                .ToList()
                .Single(el => el.GetAttribute("className") == "Tg7LZd");

            // Simulate a click event on the button
            button.InvokeMember("click");
        }

        private async static Task CrawlImages ()
        {
            var images = browser.Document.GetElementsByTagName("img").ToList()
                .Where(el => el.GetAttribute("className").StartsWith("rg_i"))
                .Take(50)
                .ToList();

            var covers = new List<Cover>();

            for (int i = 0; i < images.Count; i++)
            {
                try
                {
                    HtmlElement image = images[i];

                    // We want square images
                    if (image.ClientRectangle.Width != image.ClientRectangle.Height || image.ClientRectangle.Width < 100) continue;


                    // Open right pane viewer
                    image.InvokeMember("click");
                    await Task.Delay(350);

                    // Retreive new added attribute value and extract image url
                    string href = image.Parent?.Parent?.GetAttribute("href");
                    string encoded = href?.Replace("/imgres?imgurl=", "")?.Split('&')?[0];

                    if (href == null) continue;

                    var cover = new Cover(
                        rank: i + 1,
                        url: encoded.DecodeUrl()
                    );

                    covers.Add(cover);

                    if (covers.Count == 20) break;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.ToString(), "Bof..");
                }
            }

            SearchCompleted?.Invoke(covers);
        }

        private async static void DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
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

                    await CrawlImages();
                }
            }
            catch(Exception ex)
            {
                string message = "Une erreur est survenue, veuillez contacter l'éditeur avec une capture d'écran de ce message ainsi que le fichier qui a provoqué l'erreur.\n\n" +
                    ex.Message + "\n" +
                    ex.ToString();

                MessageBox.Show(message, "🤷‍♂️ Erreur " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
