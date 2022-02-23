using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Lottie;
using Microsoft.UI.Xaml.Controls;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Helpers;
using WinstaNext.ViewModels;
using WinstaNext.ViewModels.Media;
using WinstaNext.Views;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Media
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaMediaPresenterUC : UserControl
    {
        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          "Media",
          typeof(InstaMedia),
          typeof(InstaMediaPresenterUC),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnMediaChanged))]
        public InstaMedia Media
        {
            get { return (InstaMedia)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); ViewModel.Media = value; }
        }

        public InstaMediaPresenterUCViewModel ViewModel { get; private set; } = new InstaMediaPresenterUCViewModel();

        public InstaUserShort Me { get; private set; }

        public InstaMediaPresenterUC()
        {
            this.InitializeComponent();
            Me = App.Container.GetService<InstaUserShort>();
        }

        ~InstaMediaPresenterUC()
        {
            Me = null;
            Media = null;
            ViewModel = null;
        }

        private void OnMediaChanged()
        {
            ViewModel.ImagePresenterLoaded = 
                ViewModel.VideoPresenterLoaded = 
                    ViewModel.CarouselPresenterLoaded = false;
            switch (Media.MediaType)
            {
                case InstaMediaType.Image:
                    ViewModel.ImagePresenterLoaded = true;
                    break;
                case InstaMediaType.Video:
                    ViewModel.VideoPresenterLoaded = true;
                    break;
                case InstaMediaType.Carousel:
                    ViewModel.CarouselPresenterLoaded = true;
                    break;
                default:
                    break;
            }
        }

        private void Presenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var parentelement = (ViewModel.NavigationService.Content as FrameworkElement);
            FrameworkElement targetElement = null;
            switch (Media.MediaType)
            {
                case InstaMediaType.Image:
                    targetElement = (imagePresenter);
                    break;
                case InstaMediaType.Video:
                    targetElement = (videoPresenter);
                    break;
                case InstaMediaType.Carousel:
                    targetElement = (carouselPresenter);
                    carouselPresenter.SetFlipViewSize();
                    break;
            }
            if (targetElement == null) return;
            if (string.IsNullOrEmpty(Media.Height)) return;
            var minwidth = parentelement.ActualWidth < e.NewSize.Width ?
                parentelement.ActualWidth : e.NewSize.Width;
            var s = ControlSizeHelper.CalculateSizeInBox(Media.Width, int.Parse(Media.Height),
                parentelement.ActualHeight - 150, minwidth);
            targetElement.Width = s.Width;
            targetElement.Height = s.Height;
        }
    }
}
