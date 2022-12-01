using InstagramApiSharp.Classes.Models;
using LottieUWP;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using WinstaCore;
using WinstaCore.Helpers;
using WinstaMobile.ViewModels.Media;
using WinstaNext.Helpers;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Media
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaMediaPresenterUC : UserControl
    {
        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          nameof(Media),
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

        //~InstaMediaPresenterUC()
        //{
        //    Me = null;
        //    ViewModel = null;
        //    if (Dispatcher != null && Dispatcher.HasThreadAccess)
        //        Media = null;
        //}

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
                    if (eventRegistered)
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
                    carouselPresenter.Gallery.SelectionChanged += Gallery_SelectionChanged;
                    eventRegistered = true;
                    break;
                default:
                    break;
            }
            Presenter_SizeChanged(null, null);
        }

        private void Gallery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ApplicationSettingsManager.Instance.GetAutoPlay() || !Media.Play) return;
            var gallery = carouselPresenter.Gallery;
            for (int i = 0; i < gallery.Items.Count; i++)
            {
                var item = (FlipViewItem)gallery.ContainerFromIndex(i);
                if (item == null) continue;
                if (item.ContentTemplateRoot is not InstaMediaVideoPresenterUC videoPresenter) continue;
                videoPresenter.mediaPlayer.Pause();
            }
            var fvi = (FlipViewItem)gallery.ContainerFromIndex(gallery.SelectedIndex);
            if (fvi == null) return;
            if (fvi.ContentTemplateRoot is not InstaMediaVideoPresenterUC videoPresenterUC) return;
            videoPresenterUC.mediaPlayer.Play();
        }

        void HandleVideoPlayback(InstaMediaVideoPresenterUC videoPresenter)
        {
            if (videoPresenter == null) return;
            if (!Media.Play)
                videoPresenter.mediaPlayer.Pause();
            else videoPresenter.mediaPlayer.Play();
        }

        void HandleCarouselVideos()
        {
            if (!Media.Play)
            {
                for (int i = 0; i < Media.Carousel.Count; i++)
                {
                    var container = carouselPresenter.Gallery.ContainerFromIndex(i);
                    var fvi = (FlipViewItem)container;
                    if (container == null || fvi == null) continue;
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
            if (Media == null) return;
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
            var minwidth = parentelement.ActualWidth;
            if (e != null)
                minwidth = parentelement.ActualWidth < e.NewSize.Width ?
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

        private void videoPresenter_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (Media.Play && ApplicationSettingsManager.Instance.GetAutoPlay())
                videoPresenter.mediaPlayer.Play();
        }

        private async void AnimationPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            LottieUWP.LottieAnimationView lottieAnimationView = (LottieUWP.LottieAnimationView)sender;
            if (lottieAnimationView.Name == nameof(LikeAnimationPlayer))
            {
                await LoadAnimation(lottieAnimationView,
                    await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Lottie/like.json", UriKind.RelativeOrAbsolute)));
            }
            else
            {
                await LoadAnimation(lottieAnimationView,
                    await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Lottie/Dislike.json", UriKind.RelativeOrAbsolute)));
            }
        }

        async Task LoadAnimation(LottieAnimationView LottieAnimationView, StorageFile file)
        {
            if (file != null)
            {
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    await LottieAnimationView.SetAnimationAsync(new JsonReader(new StreamReader(stream, Encoding.UTF8)), file.Name);
                }
                LottieAnimationView.PlayAnimation();
            }
        }
    }
}
