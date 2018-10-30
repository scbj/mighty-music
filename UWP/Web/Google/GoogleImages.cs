using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace UWP.Web.Google
{
    public class GoogleImages
    {
        private WebView webView;
        private TaskCompletionSource<List<string>> tcs;

        // Scripts
        private string parseHtmlScript;

        const string BaseUrl = "https://images.google.com";
        const string SearchUrl = "https://www.google.fr/search?tbm=isch&q=";

        public GoogleImages()
        {
            webView = new WebView(WebViewExecutionMode.SeparateThread);
            webView.LoadCompleted += LoadCompleted;

            LoadScripts();
        }

        private async void LoadScripts()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Utils/Scripts/parseHtml.js"));
                parseHtmlScript = await FileIO.ReadTextAsync(file);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to initialize the search engine", ex);
            }
        }

        public Task<List<string>> SearchImages(string query)
        {
            if (String.IsNullOrWhiteSpace(query) || !HasInternet())
                throw new Exception("No internet access");

            tcs = new TaskCompletionSource<List<string>>();

            webView.Navigate(new Uri(SearchUrl + Uri.EscapeDataString(query)));

            return tcs.Task;
        }

        private async void LoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("[SearchEngine] LoadCompleted to " + e.Uri.AbsoluteUri);
            List<string> imageUrls;
            await Task.Delay(600);
            try
            {
                Debug.WriteLine("[SearchEngine] Invoke parse html script...");
                string urls = await webView.InvokeScriptAsync("eval", new string[] { parseHtmlScript });
                Debug.WriteLine("[SearchEngine] End invoke.");
                imageUrls = urls.Split('\n').ToList();

                Debug.WriteLine("[SearchEngine] " + imageUrls.Count + " result(s)");
                tcs.SetResult(imageUrls);
            }
            catch (Exception ex)
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
