using SBToolkit.Uwp.UI.View;
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
using SBToolkit.Uwp.UI.ViewModel;
using UWP.ViewModels;
using Windows.Storage;
using TagLib;
using Windows.Storage.Pickers;
using System.Diagnostics;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using UWP.Models;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Composition;
using System.Numerics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicPage : Page, IView<MusicViewModel>
    {
        private ImplicitAnimationCollection _implicitAnimations;

        public MusicPage() => InitializeComponent();

        public MusicViewModel ViewModel => base.DataContext as MusicViewModel;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.BrowseCommand.Execute(null);
        }

        private void GridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            Visual elementVisual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
            if (args.InRecycleQueue)
            {
                elementVisual.ImplicitAnimations = null;
            }
            else
            {
                EnsureImplicitAnimations(args.ItemContainer);
                elementVisual.ImplicitAnimations = _implicitAnimations;
            }
        }

        private void EnsureImplicitAnimations(FrameworkElement element)
        {
            if (_implicitAnimations == null)
            {
                Compositor compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

                CubicBezierEasingFunction easeIn = compositor.CreateCubicBezierEasingFunction(new Vector2(0.215f, 0.61f), new Vector2(0.355f, 1f));

                Vector3KeyFrameAnimation offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
                offsetAnimation.Target = nameof(Visual.Offset);
                offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue", easeIn);
                offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);
                
                CompositionAnimationGroup animationGroup = compositor.CreateAnimationGroup();
                animationGroup.Add(offsetAnimation);

                _implicitAnimations = compositor.CreateImplicitAnimationCollection();
                _implicitAnimations[nameof(Visual.Offset)] = animationGroup;
            }
        }
    }
}
