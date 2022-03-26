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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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
    }
}
