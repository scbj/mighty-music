using SBToolkit.Uwp.UI.Observable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.ViewModels
{
    public class DragAndDropViewModel : ObservableObject
    {
        bool _isLoadingContent;

        public bool IsLoadingContent
        {
            get => _isLoadingContent;
            set => Set(ref _isLoadingContent, value);
        }
    }
}
