using Microsoft.Toolkit.Mvvm.Input;
using System;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore;
using WinstaCore.Services;
using Microsoft.Extensions.DependencyInjection;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Directs.MessageContainer
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
            var NavigationService = AppCore.Container.GetService<NavigationService>();
            var SingleInstaMediaView = AppCore.Container.GetService<ISingleInstaMediaView>();
            NavigationService.Navigate(SingleInstaMediaView, DirectItem.XmaMediaShare[0].TargetUrl);
        }

        protected override void OnDirectItemChanged()
        {
            base.OnDirectItemChanged();
            if (DirectItem.XmaMediaShare[0].PreviewUrlMimeType.ToLower() == "image/jpeg")
            {
                //vidMedia.Visibility = Visibility.Collapsed;
                imgMedia.Source = new BitmapImage(new(DirectItem.XmaMediaShare[0].PreviewUrlInfo.Uri));
            }
            else
            {
                imgMedia.Source = new BitmapImage(new(DirectItem.XmaMediaShare[0].PreviewUrlInfo.Uri));
                //imgMedia.Visibility = Visibility.Collapsed;
                //vidMedia.PosterSource = new BitmapImage(new(DirectItem.XmaMediaShare[0].PreviewUrlInfo.Uri));
                //vidMedia.SetPlaybackSource(
                //    MediaSource.CreateFromUri(
                //        new Uri(DirectItem.XmaMediaShare[0].PlayableUrl, UriKind.RelativeOrAbsolute)));
            }
        }

        private void imgMedia_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            NavigateToMediaCommand.Execute(null);
        }
    }
}
