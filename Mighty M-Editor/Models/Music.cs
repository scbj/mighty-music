using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Mighty_M_Editor.Models
{
	public abstract class Music
	{
		public string Title { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public BitmapImage Cover { get; set; }

		public string GetName()
		{
			return String.Format("{0} - {1}", Artist, Title);
		}
	}
}
