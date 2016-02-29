using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Mighty_M_Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mighty_M_Editor.Views
{
	/// <summary>
	/// Logique d'interaction pour MightyView.xaml
	/// </summary>
	public partial class MightyView : MetroWindow, IView<Mighty>
	{
		// Properties
		public Mighty ViewModel { get; set; }
		object IView.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (Mighty)value; }
		}


		// Constructors
		public MightyView()
		{
			DataContext = ViewModel = Mighty.Instance;
			InitializeComponent();
			ViewModel.View = this;

			this.DragEnter += MightyView_DragEnter;
			this.DragLeave += (e, sender) => { ViewModel.IsBusy = false; };
			this.Drop += MightyView_Drop;
			this.Closing += (sender, e) => Filter.Save("filters.txt", ViewModel.Filters);
		}


		// Methods
		private async void CreatePlaylist(string[] files)
		{
			this.Activate();

			this.MetroDialogOptions.ColorScheme = MahApps.Metro.Controls.Dialogs.MetroDialogColorScheme.Accented;
			var result = await this.ShowInputAsync("Création d'une playlist", "Quel nom voulez-vous donner à la nouvelle playlist ?");

			if (result == null)	// User pressed Cancel
				return;

			System.IO.File.WriteAllText(result + ".m3u", String.Join(Environment.NewLine, files));
			System.Diagnostics.Process.Start(System.IO.Directory.GetCurrentDirectory());
		}

		private void MightyView_DragEnter(object sender, DragEventArgs e)
		{
			ViewModel.IsBusy = true;
			if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
				e.Effects = DragDropEffects.Link;
		}

		private void Hyperlink_Click(object sender, RoutedEventArgs e)
		{
			var flyout = this.Flyouts.Items[0] as Flyout;
			if (flyout == null)
				return;

			flyout.IsOpen = !flyout.IsOpen;
		}

		private void MightyView_Drop(object sender, DragEventArgs e)
		{
			bool auto = Keyboard.IsKeyDown(Key.LeftCtrl);
			ViewModel.IsBusy = false;

			bool isFile = e.Data.GetDataPresent(DataFormats.FileDrop);
			bool isUrl = e.Data.GetDataPresent(DataFormats.Text);

			if (isUrl)
			{
				ViewModel.CoverUri = e.Data.GetData(DataFormats.Text).ToString();
				ViewModel.MusicInfo = null;
			}
			else if (isFile)
			{
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
				if (files.Length > 1)
				{
					CreatePlaylist(files);
					auto = false;
				}
				else
				{
					ViewModel.MusicInfo = null;
					string name = System.IO.Path.GetFileNameWithoutExtension(files.First());
					ViewModel.Name = name;
					Clipboard.SetText(name);
				}
			}
			// Forces update ALL binding
			CommandManager.InvalidateRequerySuggested();

			if (auto)
				if (ViewModel.GenerateCommand.CanExecute(null)) ViewModel.GenerateCommand.Execute(null);
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			var text = (sender as TextBox).Text;

			if (!String.IsNullOrWhiteSpace(text))
				Clipboard.SetText(text);
		}
	}	
}
