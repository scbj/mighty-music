using Mighty_M_Editor.Models;
using Mighty_M_Editor.ViewModels;
using Mighty_M_Editor.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mighty_M_Editor
{
	public class Mighty : ViewModelBase, IViewModel<MightyView>
	{
		// Fields
		private static Mighty instance;
		private bool isBusy;
		private string coverUri;
		private string name;
		private ObservableCollection<Filter> filters;
		private MusicInfo musicInfo;

		private ICommand generateCommand;


		// Properties
		public static Mighty Instance
		{
			get
			{
				if (instance == null)
					instance = new Mighty();

				return instance;
			}
		}
		public MightyView View { get; set; }
		object IViewModel.View
		{
			get { return View; }
			set { View = (MightyView)value; }
		}
		public bool IsBusy
		{
			get { return isBusy; }
			set
			{
				isBusy = value;
				OnPropertyChanged("IsBusy");
			}
		}
		public string CoverUri
		{
			get { return coverUri; }
			set
			{
				this.coverUri = value;
				OnPropertyChanged("CoverUri");
			}
		}
		public string Name
		{
			get { return name; }
			set
			{
				this.name = value;
				OnPropertyChanged("Name");
				OnPropertyChanged("NameContentNoMatch");
			}
		}
		public bool NameContentNoMatch
		{
			get
			{
				if (String.IsNullOrWhiteSpace(Name))
					return false;

				var result = Name.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)
					.Where(s => !Filters.Any(f => f.Value == s)).ToArray();

				if (result.Length == 2)
					return false;

				return true;
			}
		}
		public ObservableCollection<Filter> Filters
		{
			get { return filters; }
			set
			{
				this.filters = value;
				OnPropertyChanged("Filters");
			}
		}
		public MusicInfo MusicInfo
		{
			get { return musicInfo; }
			set
			{
				this.musicInfo = value;
				OnPropertyChanged("MusicInfo");
			}
		}

		public ICommand GenerateCommand
		{
			get
			{
				if (this.generateCommand == null)
					this.generateCommand = new RelayCommand(() => this.Generate(), () => this.CanGenerate());

				return this.generateCommand;
			}
		}


		// Constructors
		public Mighty()
		{
			Filters = Filter.Load("filters.txt");
		}


		// Methods
		private bool CanGenerate()
		{
			if (!NameContentNoMatch && !String.IsNullOrWhiteSpace(Name) && !String.IsNullOrWhiteSpace(CoverUri))
				return true;

			return false;
		}

		private void Generate()
		{
			View.button_Process.Focus();

			// Generate
			try
			{
				var musicInfo = new MusicInfo(Name, CoverUri);
				musicInfo.Initialize(Filters);

				MusicInfo = musicInfo;
				Clipboard.SetText(MusicInfo.Title);

				Name = "";
				CoverUri = "";
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
	}
}
