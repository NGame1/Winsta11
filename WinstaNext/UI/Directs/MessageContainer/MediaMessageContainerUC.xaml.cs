using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
                vidMedia.Source = MediaSource.CreateFromUri(new Uri(DirectItem.Media.Videos[0].Uri, UriKind.RelativeOrAbsolute));
                vidMedia.MediaPlayer.IsMuted = true;
            }
        }
    }
}
