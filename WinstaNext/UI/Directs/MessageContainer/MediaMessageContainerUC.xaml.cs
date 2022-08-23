using InstagramApiSharp.Classes.Models;
using System;
using Windows.Media.Core;
using Windows.UI.Xaml;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore;
using Microsoft.Extensions.DependencyInjection;
using WinstaCore.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs.MessageContainer
{
    public sealed partial class MediaMessageContainerUC : MessageContainerUC
    {
        public MediaMessageContainerUC()
        {
            this.InitializeComponent();
        }

        protected override void OnDirectItemChanged()
        {
            base.OnDirectItemChanged();
            if (DirectItem.Media.MediaType == InstaMediaType.Image)
            {
                vidMedia.Visibility = Visibility.Collapsed;
            }
            else
            {
                imgMedia.Visibility = Visibility.Collapsed;
                vidMedia.SetPlaybackSource(
                    MediaSource.CreateFromUri(
                        new Uri(DirectItem.Media.Videos[0].Uri, UriKind.RelativeOrAbsolute)));
            }
        }

        private void imgMedia_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var NavigationService = AppCore.Container.GetService<NavigationService>();
            var ImageViewerPage = AppCore.Container.GetService<IImageViewerPage>();
            NavigationService.Navigate(ImageViewerPage, DirectItem.Media.Images[0].Uri);
        }
    }
}
