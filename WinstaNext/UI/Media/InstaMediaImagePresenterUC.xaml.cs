using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System;
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
    public sealed partial class InstaMediaImagePresenterUC : UserControl
    {
        public static readonly DependencyProperty CarouselItemProperty = DependencyProperty.Register(
             nameof(CarouselItem),
             typeof(InstaCarouselItem),
             typeof(InstaMediaImagePresenterUC),
             new PropertyMetadata(null));

        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          nameof(Media),
          typeof(InstaMedia),
          typeof(InstaMediaImagePresenterUC),
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

        public InstaMediaImagePresenterUC()
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
                img.Source = new BitmapImage(new Uri(CarouselItem.Images[0].Uri));
            }
            else
            {
                if (Media != null && Media.MediaType == InstaMediaType.Image)
                    img.Source = new BitmapImage(new Uri(Media.Images[0].Uri));
            }
        }
    }
}
