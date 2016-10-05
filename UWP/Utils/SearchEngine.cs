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
        private static WebView webView;
        private static string query;

        const string GoogleImageUrl = "https://images.google.com";
        const string GoogleSearchUrl = "https://www.google.com/search?";


        public static event Action<List<string>> SearchCompleted;

        public static void Initialize()
        {
            webView = new WebView(WebViewExecutionMode.SeparateThread);
            webView.LoadCompleted += LoadCompleted;
        }

        public static void SearchImages(string query)
        {
            if (String.IsNullOrWhiteSpace(query) && !HasInternet())
                throw new Exception("No internet access");

            SearchEngine.query = query;
            webView.Navigate(new Uri(GoogleImageUrl));
        }

        private static async void LoadCompleted(object sender, NavigationEventArgs e)
        {
            Debug.WriteLine("[SearchEngine] LoadCompleted to " + e.Uri.AbsoluteUri);
            try
            {
                if (e.Uri.AbsoluteUri.Contains(GoogleImageUrl))
                {
                    // Populate search input
                    Debug.WriteLine("[SearchEngine] Invoke populate input...");
                    await webView.InvokeScriptAsync("eval", new string[] { "document.getElementById('lst-ib').value = '"+ query +"'" });
                    await Task.Delay(400);
                    // Click on search button
                    Debug.WriteLine("[SearchEngine] Invoke button click...");
                    await webView.InvokeScriptAsync("eval", new string[] { "document.getElementsByName('btnG')[0].click()" });
                }
                else if (e.Uri.AbsoluteUri.Contains(GoogleSearchUrl))
                {
                    List<string> imageUrls;
                    await Task.Delay(800);
                    StorageFile scriptFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Utils/Scripts/parseHtml.js"));
                    string script = await FileIO.ReadTextAsync(scriptFile);
                    Debug.WriteLine("[SearchEngine] Invoke parse html script...");
                    string urls = await webView.InvokeScriptAsync("eval", new string[] { script });
                    Debug.WriteLine("[SearchEngine] End invoke.");
                    imageUrls = urls.Split(' ').ToList();
                    for (int i = 0; i < imageUrls.Count; i++)
                        imageUrls[i] = Uri.UnescapeDataString(imageUrls[i].Split('&')[0]);

                    SearchCompleted?.Invoke(imageUrls);
                }
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
