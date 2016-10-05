using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP.Utils;
using UWP.Views;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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

            SearchEngine.Initialize();

            RootFrame.Navigate(typeof(DragAndDropPage));
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            RootFrame.Navigated += RootFrame_Navigated;
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.AppViewBackButtonVisibility = RootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (RootFrame.CanGoBack)
            {
                RootFrame.GoBack();
                e.Handled = true;
            }
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            RootSplitView.IsPaneOpen = !RootSplitView.IsPaneOpen;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button != null && button.IsChecked.HasValue && button.IsChecked.Value)
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
                if (pageType != null && RootFrame.CurrentSourcePageType != pageType)
                    RootFrame.Navigate(pageType);
            }
        }
    }
}
