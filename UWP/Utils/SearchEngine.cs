using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

namespace UWP.Utils
{
    public class SearchEngine
    {
        private static WebView webView;

        const string GoogleImageUrl = "https://images.google.com";
        const string GoogleSearchResult = "https://www.google.com/search?";

        public static void Initialize()
        {
            return;
            Test();
            return;

            webView = new WebView(WebViewExecutionMode.SeparateThread);
            webView.NavigationCompleted += WebView_NavigationCompleted;

            webView.Navigate(new Uri(GoogleImageUrl));
        }

        private async static void Test()
        {
            return;
            const string apiKey = "AIzaSyBXZigV3Tow5lYjlIMZN8cUxG7rzlWmASE";
            const string searchEngineId = "005418149810438865353:oyu9fu0jq0c";
            const string query = "Le Premier Bonheur Du Jour (MrCØ Remix) soundcloud";
            var customSearchService = new CustomsearchService(new BaseClientService.Initializer() { ApiKey = apiKey });
            CseResource.ListRequest listRequest = customSearchService.Cse.List(query);
            listRequest.Cx = searchEngineId;
            Search search = await listRequest.ExecuteAsync();
            foreach (var item in search.Items)
            {
                Debug.WriteLine(item.Link);
            }
        }

        private async static void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
        }
    }
}
