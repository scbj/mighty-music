using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            rootFrame.Navigate(typeof(DragAndDropPage));            
        }

        private void SplitViewHamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void SplitViewRadioButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button != null)
            {
                Type pageType = null;
                switch (button.Content.ToString())
                {
                    case "Glisser-déposer":
                        pageType = typeof(DragAndDropPage);
                        break;
                    case "Non traitée(s)":
                        pageType = typeof(UntreatedPage);
                        break;
                    case "Toutes les musiques":
                        pageType = typeof(MusicPage);
                        break;
                    case "Paramètres":
                        pageType = typeof(SettingsPage);
                        break;
                }
                if (pageType != null && rootFrame.CurrentSourcePageType != pageType)
                    rootFrame.Navigate(pageType);
            }
        }
    }
}
