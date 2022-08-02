using Abstractions.Navigation;
using Abstractions.Stories;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using WinstaCore;
using WinstaCore.Services;
using WinstaNext.Views.Stories;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs.MessageContainer
{
    public sealed partial class StoryShareMessageContainerUC : MessageContainerUC
    {
        public StoryShareMessageContainerUC()
        {
            this.InitializeComponent();
        }

        protected override void OnDirectItemChanged()
        {
            if (DirectItem.StoryShare.ReelId != null)
            {
                imgMedia.Source = new BitmapImage(new(DirectItem.StoryShare.Media.Images[0].Uri, UriKind.RelativeOrAbsolute));
            }

            if (string.IsNullOrEmpty(DirectItem.StoryShare.Text) && !string.IsNullOrEmpty(DirectItem.StoryShare.Message))
                txtText.Text = DirectItem.StoryShare.Message;
            else txtStack.Visibility = Visibility.Collapsed;

            base.OnDirectItemChanged();
        }

        private void imgMedia_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var NavigationService = AppCore.Container.GetService<NavigationService>();
            var story = new WinstaStoryItem(DirectItem.StoryShare);
            NavigationService.Navigate(typeof(StoryCarouselView),
                new StoryCarouselViewParameter(story));
        }
    }
}
