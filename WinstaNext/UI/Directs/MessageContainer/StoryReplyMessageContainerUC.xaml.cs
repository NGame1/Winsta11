using System;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs.MessageContainer
{
    public sealed partial class StoryReplyMessageContainerUC : MessageContainerUC
    {
        public StoryReplyMessageContainerUC()
        {
            this.InitializeComponent();
        }

        protected override void OnDirectItemChanged()
        {
            if (DirectItem.ReelShareMedia.Media.Images.Count != 0)
            {
                imgMedia.Source = new BitmapImage(new(DirectItem.ReelShareMedia.Media.Images[0].Uri, UriKind.RelativeOrAbsolute));
            }
            base.OnDirectItemChanged();
        }
    }
}
