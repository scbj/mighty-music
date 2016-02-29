using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_Music.Utils
{
    public static class Filter
    {
        const string path = "filters.ini";
        private static string[] filters;

        public static string[] Filters
        {
            get
            {
                if (filters == null)
                    filters = GetFilters();
                return filters;
            }
        }

        private static string[] GetFilters()
        {
            if (!File.Exists(path))
                File.Create(path);

            return File.ReadAllLines(path);
        }

        public static void Save(string[] filters) => File.WriteAllLines(path, filters);

        public static string Flush(string original)
        {
            foreach (var filter in Filters)
                original = original.Replace(filter, "");

            return original.Trim();
        }
    }
}
