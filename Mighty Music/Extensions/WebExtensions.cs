using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mighty_Music.Extensions
{
    public static class WebExtensions
    {
        public static Task NavigateAsync(this WebBrowser webBrowser, string url)
        {
            webBrowser.Navigate(url);
            return Wait(webBrowser);
        }

        public static Task Wait(this WebBrowser webBrowser)
        {
            var tcs = new TaskCompletionSource<bool>();
            webBrowser.DocumentCompleted += (s, args) => tcs.TrySetResult(true);
            return tcs.Task;
        }

        public static List<HtmlElement> ToList(this HtmlElementCollection collection) => collection.Cast<HtmlElement>().ToList();

        public static string DecodeUrl(this string encoded)
        {
            return encoded.Replace("%253F", "?")
                .Replace("%253D", "=")
                .Replace("%2525", "%");
        }
    }
}
