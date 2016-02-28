using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace Mighty_M_Helper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Queue<string> queueFiles;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse(object sender, RoutedEventArgs e)
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
            ResetFields();

            txtbox_currentFile.Tag = path;
            string fileName = txtbox_currentFile.Text = System.IO.Path.GetFileNameWithoutExtension(path);

            string[] splitted = fileName.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
            var filterStrings = new List<string>();
            foreach (var s in splitted)
                if (!s.StartsWith("[") || !s.EndsWith("]"))
                    filterStrings.Add(s);

            if (filterStrings.Count >= 2)
            {
                txtbox_title.Text = filterStrings[1];
                txtbox_artist.Text = filterStrings[0];
                txtbox_album.Text = filterStrings[1];
            }

            TaskScheduler scheduler = TaskScheduler.FromCurrentSynchronizationContext(); ;
            ImageSearch.GetCovers(txtbox_artist.Text, txtbox_title.Text).ContinueWith((s) => {
                lsb_covers.ItemsSource = s.Result;
            }, CancellationToken.None, TaskContinuationOptions.None, scheduler);
        }

        private void ResetFields()
        {
            txtbox_currentFile.Clear();
            txtbox_title.Clear();
            txtbox_artist.Clear();
            txtbox_album.Clear();
            lsb_covers.ItemsSource = null;
        }

        private void Apply(object sender, RoutedEventArgs e)
        {
            if (txtbox_currentFile.Text == "")
                return;

            if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                EditMP3File();
            if (queueFiles.Count > 0)
                LoadMusic(queueFiles.Dequeue());
            else
                ResetFields();
        }

        private void EditMP3File()
        {
            var file = TagLib.File.Create((string)txtbox_currentFile.Tag);
            file.Tag.Performers = new string[] { txtbox_artist.Text };
            file.Tag.Title = txtbox_title.Text;
            file.Tag.Album = txtbox_album.Text;
            
            var pictures = new TagLib.IPicture[] { new TagLib.Picture(GetCover()) };
            file.Tag.Pictures = pictures;
            file.Save();
        }

        private string GetCover()
        {
            var bytes = new WebClient().DownloadData((lsb_covers.SelectedItem as Cover).Url);
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(bytes); ;
            bi.DecodePixelHeight = 500;
            bi.DecodePixelWidth = 500;
            bi.EndInit();
            bi.Freeze();

            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bi));

            string path = @"C:\Users\Sacha\Music\Pochettes\" + String.Format("{0} - {1}", txtbox_artist.Text, txtbox_title.Text) + ".jpg";

            using (var filestream = new FileStream(path, FileMode.Create))
                encoder.Save(filestream);

            return path;
        }
    }
}
