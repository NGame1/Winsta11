using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Media
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaMediaVideoPresenterUC : UserControl
    {
        public static readonly DependencyProperty CarouselItemProperty = DependencyProperty.Register(
             "CarouselItem",
             typeof(InstaCarouselItem),
             typeof(InstaMediaVideoPresenterUC),
             new PropertyMetadata(null));

        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          "Media",
          typeof(InstaMedia),
          typeof(InstaMediaVideoPresenterUC),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnMediaChanged))]
        public InstaCarouselItem CarouselItem
        {
            get { return (InstaCarouselItem)GetValue(CarouselItemProperty); }
            set { SetValue(CarouselItemProperty, value); }
        }

        [OnChangedMethod(nameof(OnMediaChanged))]
        public InstaMedia Media
        {
            get { return (InstaMedia)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        public event RoutedEventHandler MediaEnded;

        public InstaMediaVideoPresenterUC()
        {
            this.InitializeComponent();
        }

        Point _dragpoint;
        private void SV_ImageZoom_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                e.Handled = true;
                var thissv = (sender as ScrollViewer);
                var o = e.GetCurrentPoint(thissv);
                var isleftpress = o.Properties.IsLeftButtonPressed;
                if (isleftpress)
                {
                    CancelDirectManipulations();
                    var p = e.GetCurrentPoint(thissv);
                    var offsetX = p.Position.X - _dragpoint.X;
                    var offsetY = p.Position.Y - _dragpoint.Y;
                    thissv.ChangeView((thissv.HorizontalOffset - (offsetX * 5)), (thissv.VerticalOffset - (offsetY * 5)), null);
                    _dragpoint = p.Position;
                }
            }
        }

        private void SV_ImageZoom_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as FrameworkElement);
            _dragpoint = p.Position;
        }

        void OnMediaChanged()
        {
            if (CarouselItem != null)
            {
                mediaPlayer.PosterSource = new BitmapImage(new Uri(CarouselItem.Images[0].Uri));
                mediaPlayer.Source = new Uri(CarouselItem.Videos[0].Uri);
            }
            else
            {
                if(Media != null && Media.MediaType == InstaMediaType.Video)
                {
                    mediaPlayer.PosterSource = new BitmapImage(new Uri(Media.Images[0].Uri));
                    mediaPlayer.Source = new Uri(Media.Videos[0].Uri);
                }
            }
        }

        private void mediaPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaEnded?.Invoke(sender, e);
        }

        private async void mediaPlayer_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var media = Media;
            if(media == null)
            {
                var mediaPresenter = this.FindAscendant<InstaMediaPresenterUC>();
                media = mediaPresenter.Media;
            }
            var liked = media.HasLiked;
            await Task.Delay(500);
            if (liked != media.HasLiked) return;
            var pos = e.GetPosition((UIElement)sender).Y;
            if (pos < mediaPlayer.ActualHeight - 60)
            {
                mediaPlayer.IsMuted = !mediaPlayer.IsMuted;
            }
        }
    }
}
