using Mighty_Music.Views;
using MVVM.Pattern.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_Music.ViewModels
{
    public class MainViewModel : ViewModelBase, IViewModel<MainView>
    {
        // Properties
        public MainView View { get; set; }
        object IViewModel.View
        {
            get { return View; }
            set { View = (MainView)value; }
        }
    }
}
