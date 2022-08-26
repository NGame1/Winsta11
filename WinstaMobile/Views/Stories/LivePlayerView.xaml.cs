using Abstractions.Stories;
using FFmpegInteropX;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Services;
using WinstaNext.Helpers;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Stories
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
          nameof(StoryRoot),
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
