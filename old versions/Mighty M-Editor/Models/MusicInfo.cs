using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Mighty_M_Editor.Models
{
	public sealed class MusicInfo : Music, INotifyPropertyChanged
	{
		public string Name { get; set; }
		public string CoverUrl { get; set; }

		public MusicInfo(string name, string coverUrl)
		{
			this.Name = name;
			this.CoverUrl = coverUrl;
		}

		public void Initialize(IEnumerable<Filter> filters)
		{
			try
			{
				var result = Name.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
				.Where(s => !filters.Any(f => f.Value == s)).ToArray();

				for (int i = 0; i < result.Length; i++)
				{
					foreach (var f in filters)
					{
						result[i] = result[i].Replace(f.Value, "").Trim();
					}
				}

				Title = result[1];
				Artist = result[0];
				Album = result[1];
			}
			catch(Exception ex)
			{
				throw new FormatException("Incorrect format, please follow the model '{ARTIST} - {TITLE}'", ex);
			}
			

			Task.Run(() => LoadImage());
		}

		public void LoadImage()
		{
			var bytes = new WebClient().DownloadData(CoverUrl);
			var bi = new BitmapImage();
			bi.BeginInit();
			bi.StreamSource = new MemoryStream(bytes); ;
			bi.DecodePixelHeight = 500;
			bi.DecodePixelWidth = 500;
			bi.EndInit();
			bi.Freeze();

			Cover = bi;
			OnPropertyChanged("Cover");

			JpegBitmapEncoder encoder = new JpegBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bi));

			using (var filestream = new FileStream(@"D:\Music\Pochettes d'album\" + base.GetName() + ".jpg", FileMode.Create))
				encoder.Save(filestream);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
