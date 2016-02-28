using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_M_Editor.Models
{
	public class Filter
	{
		public string Value { get; set; }

		public Filter() { }

		public Filter(string value)
		{
			this.Value = value;
		}

		public override string ToString()
		{
			return Value;
		}

		public static ObservableCollection<Filter> Load(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException("File not found 'filters.txt'");

			var lines = File.ReadAllLines(path).Select(s => new Filter(s));
			return new ObservableCollection<Filter>(lines);
		}

		public static void Save(string path, IEnumerable<Filter> filters)
		{
			File.WriteAllLines(path, filters.Select(f => f.Value));
		}
	}
}
