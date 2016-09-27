using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.Controls
{
    public sealed partial class InputContentDialog : ContentDialog
    {
        public InputContentDialog()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public InputContentDialog(string title) : this()
        {
            this.Title = title;
        }

        public string Input { get { return input.Text; } }
    }
}
