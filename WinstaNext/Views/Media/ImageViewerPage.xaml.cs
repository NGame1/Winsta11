using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Media
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImageViewerPage : Page
    {
        public ImageViewerPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not string uri)
                throw new ArgumentException(nameof(e.Parameter));
            img.Source = new BitmapImage(new(uri, UriKind.RelativeOrAbsolute));
            base.OnNavigatedTo(e);
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

        private void img_ImageOpened(object sender, RoutedEventArgs e)
        {
            PR.Visibility = Visibility.Collapsed;
        }
    }
}
