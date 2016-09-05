using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mighty_Music.Extensions
{
    public static class WebBrowserExtension
    {
        public static List<HtmlElement> ToList(this HtmlElementCollection collection) => collection.Cast<HtmlElement>().ToList();

        public static string DecodeUrl(this string encoded)
        {
            encoded = System.Web.HttpUtility.UrlDecode(encoded);
            return encoded.Replace("%253F", "?")
                .Replace("%253D", "=")
                .Replace("%2525", "%");
        }
    }
}
