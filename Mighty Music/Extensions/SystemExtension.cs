using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_Music.Extensions
{
    public static class SystemExtension
    {
        /// <summary>
        /// Retourne une nouvelle chaîne dans laquelle toutes les occurences des chaînes spécifiée dans l'instance actuelle sont remplacées par une autre chaîne spécifiée.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="oldValues">Chaînes à remplacer.</param>
        /// <param name="newValue">Chaîne permettant de rempalcer toutes les occurrences des chaînes de oldValues.</param>
        /// <returns></returns>
        public static string Replace(this string value, IEnumerable<string> oldValues, string newValue)
        {
            var result = new StringBuilder(value);
            foreach (string s in oldValues)
                result.Replace(s, newValue);

            return result.ToString();
        }
        public static string Replace(this string value, IEnumerable<Tuple<string, string>> values)
        {
            var result = new StringBuilder(value);
            foreach (var t in values)
                result.Replace(t.Item1, t.Item2);

            return result.ToString();
        }
        public static string Remove(this string value, IEnumerable<string> values) => value.Replace(values, "");
        public static string Remove(this string value, IEnumerable<char> values) => value.Replace(values.CastToString(), "");
        
        public static IEnumerable<string> CastToString(this IEnumerable<char> chars) => chars.Select(c => c.ToString());
    }
}
