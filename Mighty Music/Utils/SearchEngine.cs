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
            try
            {
                if (e.Url.AbsoluteUri.Contains(GOOGLE_IMAGE_URL))
                {
                    browser.Document.GetElementById("lst-ib").InnerText = query;
                    await Task.Delay(200);
                    browser.Document.All.GetElementsByName("btnG").ToList().Single().InvokeMember("click");
                }
                else if (e.Url.AbsoluteUri.Contains(GOOGLE_SEARCH_RESULT))
                {
                    await Task.Delay(200);
                    var covers = new List<Cover>();
                    Clipboard.SetText(browser.Document.Body.OuterHtml);
                    var divs = browser.Document.GetElementById("rg_s").Children.ToList().Where(he => he.GetAttribute("className") == "rg_di rg_el ivg-i").ToList();
                    for (int i = 0; i < divs.Count; i++)
                    {
                        var span = divs[i].GetElementsByTagName("span").ToList().Single(s => s.InnerText.Contains('×'));
                        var sizes = span.InnerText.Split('-')[0].Trim().Split('×');
                        int x = int.Parse(sizes[0].Trim()),
                            y = int.Parse(sizes[1].Trim());

                        if (x == y && x >= 300)
                        {
                            var href = divs[i].GetElementsByTagName("a").ToList().Single().GetAttribute("href");
                            covers.Add(new Cover(i + 1, href.Replace(/*"https://www.google.com*/"/imgres?imgurl=", "").Split('&')[0].DecodeUrl()));
                        }
                        if (covers.Count > 10 || i > 20)
                            break;
                    }

                    SearchCompleted?.Invoke(covers);
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
