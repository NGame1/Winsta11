using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using WinstaNext.Services;
using WinstaNext.Views.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs.MessageContainer
{
    public sealed partial class XmaMediaShareMessageContainerUC : MessageContainerUC
    {
        public RelayCommand NavigateToMediaCommand { get; set; }
        public XmaMediaShareMessageContainerUC()
        {
            this.InitializeComponent();
            NavigateToMediaCommand = new(NavigateToMedia);
        }

        void NavigateToMedia()
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(SingleInstaMediaView), DirectItem.XmaMediaShare[0].TargetUrl);
        }

        protected override void OnDirectItemChanged()
        {
            base.OnDirectItemChanged();
            if (DirectItem.XmaMediaShare[0].PreviewUrlMimeType.ToLower() == "image/jpeg")
            {
                vidMedia.Visibility = Visibility.Collapsed;
                imgMedia.Source = new BitmapImage(new(DirectItem.XmaMediaShare[0].PreviewUrlInfo.Uri));
            }
            else
            {
                imgMedia.Visibility = Visibility.Collapsed;
                vidMedia.PosterSource = new BitmapImage(new(DirectItem.XmaMediaShare[0].PreviewUrlInfo.Uri));
                vidMedia.SetPlaybackSource(
                    MediaSource.CreateFromUri(
                        new Uri(DirectItem.XmaMediaShare[0].PlayableUrl, UriKind.RelativeOrAbsolute)));
            }
        }
    }
}
