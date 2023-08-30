using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using Windows.Media.Core;
using Windows.UI.Xaml;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore;
using WinstaCore.Services;
using Microsoft.Extensions.DependencyInjection;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Directs.MessageContainer
{
    public sealed partial class IGTVShareMessageContainerUC : MessageContainerUC
    {
        public RelayCommand NavigateToMediaCommand { get; set; }

        public IGTVShareMessageContainerUC()
        {
            this.InitializeComponent();
            NavigateToMediaCommand = new(NavigateToMedia);
        }

        void NavigateToMedia()
        {
            var NavigationService = AppCore.Container.GetService<NavigationService>();
            var SingleInstaMediaView = AppCore.Container.GetService<ISingleInstaMediaView>();
            NavigationService.Navigate(SingleInstaMediaView, DirectItem.FelixShareMedia);
        }

        protected override void OnDirectItemChanged()
        {
            base.OnDirectItemChanged();
            if (DirectItem.FelixShareMedia.MediaType == InstaMediaType.Image)
            {
                //vidMedia.Visibility = Visibility.Collapsed;
            }
            else
            {
                //imgMedia.Visibility = Visibility.Collapsed;
                //vidMedia.SetPlaybackSource(
                //    MediaSource.CreateFromUri(
                //        new Uri(DirectItem.FelixShareMedia.Videos[0].Uri, UriKind.RelativeOrAbsolute)));
            }
        }

        private void imgMedia_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            NavigateToMediaCommand.Execute(null);
        }
    }
}
