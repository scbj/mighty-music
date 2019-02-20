using Microsoft.Win32;
using Mighty_Music.Models;
using Mighty_Music.Utils;
using Mighty_Music.Views;
using MVVM.Pattern.ViewModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Mighty_Music.ViewModels
{
    public class MainViewModel : ViewModelBase, IViewModel<MainView>
    {
        // Fields
        private Queue<string> queueFiles = new Queue<string>();
        private MusicFileViewModel current;
        private bool isBusy;
        private bool isSearching;
        private string busyName;
        private List<Cover> covers;
        private Cover selectedCover;

        // Properties
        public MainView View { get; set; }
        object IViewModel.View
        {
            get { return View; }
            set { View = (MainView)value; }
        }
        public MusicFileViewModel CurrentMusicFile
        {
            get { return current; }
            set
            {
                current = value;
                if (current == null)
                    View.lsb_covers.ItemsSource = null;
                OnPropertyChanged(nameof(CurrentMusicFile));
            }
        }
        public string WindowTitle => $"Mighty Music - {Assembly.GetExecutingAssembly().GetName().Version}";
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
            }
        }
        public bool IsSearching
        {
            get { return isSearching; }
            set
            {
                isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
            }
        }
        public string BusyName
        {
            get { return busyName; }
            set
            {
                busyName = value;
                OnPropertyChanged(nameof(BusyName));
            }
        }
        public int RemainingCount
        {
            get { return queueFiles == null ? 0 : queueFiles.Count; }
        }
        public List<Cover> Covers
        {
            get { return covers; }
            set
            {
                covers = value;
                OnPropertyChanged(nameof(Covers));
            }
        }
        public Cover SelectedCover
        {
            get { return selectedCover; }
            set
            {
                selectedCover = value;
                OnPropertyChanged(nameof(SelectedCover));
            }
        }
        public ICommand BrowseCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get;  }
        public ICommand SkipCommand { get; }

        public MainViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
            ApplyCommand = new RelayCommand(Apply);
            CancelCommand = new RelayCommand(Cancel);
            SkipCommand = new RelayCommand((arg) => Continue());

            SearchEngine.Initialize();
            SearchEngine.SearchCompleted += (args) =>
            {
                Covers = args;
                IsSearching = false;
            };

            NamedPipes.MessageReceived += NamedPipes_MessageReceived;
            NamedPipes.StartServer();
        }

        private void NamedPipes_MessageReceived(string path)
        {
            queueFiles.Enqueue(path);
            OnPropertyChanged(nameof(RemainingCount));
            if (current == null)
                LoadMusic(queueFiles.Dequeue());
        }

        private void Apply(object obj)
        {
            if (current != null)
            {
                BusyName = current.Name;
                IsBusy = true;

                if (SelectedCover != null)
                    current.Cover = selectedCover;

                current.Model.Save().ContinueWith((t) => { IsBusy = false; });
            }

            Continue();
        }

        private void Cancel (object obj) => Continue();

        private void Continue()
        {
            if (queueFiles?.Count > 0)
                LoadMusic(queueFiles.Dequeue());
            else
                CurrentMusicFile = null;
        }

        private void Browse(object sender)
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Sélectionnez des musiques...";
            ofd.DefaultExt = ".mp3";
            ofd.Filter = "Fichiers MP3 (*.mp3)|*.mp3";
            ofd.Multiselect = true;
            bool? result = ofd.ShowDialog();

            if (result == true)
            {
                queueFiles = new Queue<string>(ofd.FileNames);
                LoadMusic(queueFiles.Dequeue());
            }
        }

        private void LoadMusic(string path)
        {
            Covers = null;
            OnPropertyChanged(nameof(RemainingCount));
            CurrentMusicFile = new MusicFileViewModel(new MusicFile(path));

            SearchCover();
        }

        public void SearchCover()
        {
            IsSearching = true;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                string query = CurrentMusicFile.ToString() + " " + View.lsb_covers.Tag.ToString();
                SearchEngine.SearchCovers(query);
            }));
        }
    }
}
