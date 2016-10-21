using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWP.Utils
{
    public class SearchEngine
    {
        private WebView webView;
        private string query;

        const string GoogleImageUrl = "https://images.google.com";
        const string GoogleSearchUrl = "https://www.google.com/search?";
        const string GoogleImageRequestUrl = "https://www.google.nl/search?tbm=isch&q=";


        public event Action<List<string>> SearchCompleted;

        public SearchEngine()
        {
            webView = new WebView(WebViewExecutionMode.SeparateThread);
            webView.LoadCompleted += LoadCompleted;
        }

        public void SearchImages(string query)
        {
            if (String.IsNullOrWhiteSpace(query) && !HasInternet())
                throw new Exception("No internet access");

            this.query = query;
            webView.Navigate(new Uri(GoogleImageRequestUrl + Uri.EscapeDataString(query)));
        }

        private async void LoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("[SearchEngine] LoadCompleted to " + e.Uri.AbsoluteUri);
            List<string> imageUrls;
            await Task.Delay(600);
            try
            {
                StorageFile scriptFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Utils/Scripts/parseHtml.js"));
                string script = await FileIO.ReadTextAsync(scriptFile);
                Debug.WriteLine("[SearchEngine] Invoke parse html script...");
                string urls = await webView.InvokeScriptAsync("eval", new string[] { script });
                Debug.WriteLine("[SearchEngine] End invoke.");
                imageUrls = urls.Split(' ').ToList();
                for (int i = 0; i < imageUrls.Count; i++)
                    imageUrls[i] = Uri.UnescapeDataString(imageUrls[i].Split('&')[0]);

                Debug.WriteLine("[SearchEngine] " + imageUrls.Count + " result(s)");
                SearchCompleted?.Invoke(imageUrls);
            }
            catch(Exception ex)
            {
                var dialog = new MessageDialog("SearchEngine error : " + Environment.NewLine + ex.ToString());
                await dialog.ShowAsync();
            }
        }

        public static bool HasInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                    connectionProfile.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
        }
    }
}
