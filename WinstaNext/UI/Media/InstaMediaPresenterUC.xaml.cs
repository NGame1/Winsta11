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
using Windows.UI.Core;
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

        bool eventRegistered = false;
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
                    if(eventRegistered)
                    {
                        Media.PropertyChanged -= Media_PropertyChanged;
                    }
                    Media.PropertyChanged += Media_PropertyChanged;
                    eventRegistered = true;
                    break;
                case InstaMediaType.Carousel:
                    ViewModel.CarouselPresenterLoaded = true;
                    if (eventRegistered)
                    {
                        Media.PropertyChanged -= Media_PropertyChanged;
                    }
                    Media.PropertyChanged += Media_PropertyChanged;
                    eventRegistered = true;
                    break;
                default:
                    break;
            }
        }

        void HandleVideoPlayback(InstaMediaVideoPresenterUC videoPresenter)
        {
            if (!Media.Play)
                videoPresenter.mediaPlayer.Pause();
            else videoPresenter.mediaPlayer.Play();
        }

        void HandleCarouselVideos()
        {
            if(!Media.Play)
            {
                for (int i = 0; i < Media.Carousel.Count; i++)
                {
                    var container = carouselPresenter.Gallery.ContainerFromIndex(i);
                    var fvi = (FlipViewItem)container;
                    if (container == null || fvi == null) return;
                    if (fvi.ContentTemplateRoot is InstaMediaVideoPresenterUC videoPresenterUC)
                        videoPresenterUC.mediaPlayer.Pause();
                }
            }
            else
            {
                var i = carouselPresenter.Gallery.SelectedIndex;
                var container = carouselPresenter.Gallery.ContainerFromIndex(i);
                var fvi = (FlipViewItem)container;
                if (container == null || fvi == null) return;
                if (fvi.ContentTemplateRoot is InstaMediaVideoPresenterUC videoPresenterUC)
                    videoPresenterUC.mediaPlayer.Play();
            }
        }

        private void Media_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!ApplicationSettingsManager.Instance.GetAutoPlay()) return;
            if (Media.MediaType == InstaMediaType.Carousel)
                CarouselMedia_PropertyChanged(sender, e);
            else VideoMedia_PropertyChanged(sender, e);
        }

        private async void CarouselMedia_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Media.Play)) return;
            if (Dispatcher.HasThreadAccess)
                HandleCarouselVideos();
            else await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, HandleCarouselVideos);
        }

        private async void VideoMedia_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(Media.Play)) return;
            if (Dispatcher.HasThreadAccess)
                HandleVideoPlayback(videoPresenter);
            else await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleVideoPlayback(videoPresenter));
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

        void SendButtonKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.AddCommentCommand.Execute(null);
            args.Handled = true;
        }
    }
}
