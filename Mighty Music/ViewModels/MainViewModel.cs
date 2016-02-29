﻿using Microsoft.Win32;
using Mighty_Music.Models;
using Mighty_Music.Utils;
using Mighty_Music.Views;
using MVVM.Pattern.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mighty_Music.ViewModels
{
    public class MainViewModel : ViewModelBase, IViewModel<MainView>
    {
        // Fields
        private Queue<string> queueFiles;
        private MusicFile current;
        private bool isBusy;
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

        public MusicFile CurrentMusicFile
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
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
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
        public ICommand BrowseCommand { get; set; }
        public ICommand ApplyCommand { get; set; }

        public MainViewModel()
        {
            BrowseCommand = new RelayCommand(Browse);
            ApplyCommand = new RelayCommand(Apply);
        }

        private void Apply(object obj)
        {
            if (current != null && !Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                BusyName = current.Name;
                IsBusy = true;
                current.Cover = selectedCover;
                current.Save().ContinueWith((t) => { IsBusy = false; });
            }
                
            if (queueFiles.Count > 0)
                LoadMusic(queueFiles.Dequeue());
            else
                CurrentMusicFile = null;
        }

        private void Browse(object sender)
        {
            var ofd = new OpenFileDialog();
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
            CurrentMusicFile = new MusicFile(path);
            current.Initialize();

            SearchCover();
        }

        public async void SearchCover()
        {
            Covers = await SearchEngine.GetCovers(CurrentMusicFile.ToString(), View.lsb_covers.Tag.ToString());
        }
    }
}