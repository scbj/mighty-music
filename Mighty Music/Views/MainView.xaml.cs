using Mighty_Music.ViewModels;
using MVVM.Pattern.View;
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
using System.Windows.Shapes;

namespace Mighty_Music.Views
{
    /// <summary>
    /// Logique d'interaction pour MainView.xaml
    /// </summary>
    public partial class MainView : Window, IView<MainViewModel>
    {
        // Properties
        public MainViewModel ViewModel { get; set; }
        object IView.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (MainViewModel)value; }
        }

        public MainView()
        {
            DataContext = ViewModel = new MainViewModel();
            InitializeComponent();
            ViewModel.View = this;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.CurrentMusicFile != null)
                ViewModel.SearchCover();
        }
    }
}
