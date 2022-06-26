using FFmpegInteropX;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using MinistaLivePlayback.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.Streaming.Adaptive;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Helpers;
using WinstaNext.Services;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class LivePlayerView : Page
    {
        public double PageHeight { get; set; }

        public double PageWidth { get; set; }

        public static readonly DependencyProperty StoryRootProperty = DependencyProperty.Register(
          "StoryRoot",
          typeof(WinstaStoryItem),
          typeof(LivePlayerView),
          new PropertyMetadata(null));

        public WinstaStoryItem StoryRoot
        {
            get { return (WinstaStoryItem)GetValue(StoryRootProperty); }
            set { SetValue(StoryRootProperty, value); }
        }

        [OnChangedMethod(nameof(OnLiveChanged))]
        public InstaBroadcast Live
        {
            get => StoryRoot.Broadcast;
        }

        public LivePlayerView()
        {
            this.InitializeComponent();
        }

        async void OnLiveChanged()
        {
            var config = new MediaSourceConfig()
            {
                VideoDecoderMode = VideoDecoderMode.ForceFFmpegSoftwareDecoder
            };
            var ms = await FFmpegMediaSource.CreateFromUriAsync(Live.DashAbrPlaybackUrl, config);
            var source = ms.GetMediaStreamSource();
            mediaPlayerElement.SetMediaStreamSource(source);
            SetSize(ms.CurrentVideoStream);
        }

        void SetSize(VideoStreamInfo props)
        {
            var nav = App.Container.GetService<NavigationService>();
            {
                var parentFrame = (FrameworkElement)nav.Content;
                var size = ControlSizeHelper.CalculateSizeInBox(props.PixelWidth, props.PixelHeight, parentFrame.ActualHeight, parentFrame.ActualWidth);
                PageHeight = size.Height;
                PageWidth = size.Width;
                mediaPlayerElement.Height = PageHeight;
                mediaPlayerElement.Width = PageWidth;
            }
        }
    }
}
