using InstagramApiSharp.Classes.Models;
using System;
using Windows.Media.Core;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Directs.MessageContainer
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
    }
}
