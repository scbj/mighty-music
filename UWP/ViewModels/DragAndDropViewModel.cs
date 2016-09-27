using MVVM.Pattern__UWP_.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.ViewModels
{
    public class DragAndDropViewModel : ObservableObject
    {
        bool isLoadingContent;

        public bool IsLoadingContent
        {
            get { return isLoadingContent; }
            set
            {
                isLoadingContent = value;
                OnPropertyChanged(nameof(IsLoadingContent));
            }
        }
    }
}
