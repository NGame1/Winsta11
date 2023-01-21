using Abstractions.Stories;
using FFmpegInteropX;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using MinistaLivePlayback.Models;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Helpers;
using WinstaCore.Services;
using WinstaNext.Helpers;

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
        
        FFmpegMediaSource MediaSource { get; set; }
        MediaStreamSource MediaStreamSource { get; set; }

        public LivePlayerView()
        {
            this.InitializeComponent();
            CodecChecker.CodecRequired += CodecChecker_CodecRequired;
        }

        private async void CodecChecker_CodecRequired(object sender, CodecRequiredEventArgs args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                new DispatchedHandler(async () =>
                {
                    await AskUserInstallCodec(args);
                }));
        }

        async Task AskUserInstallCodec(CodecRequiredEventArgs args)
        {
            // show message box to user

            // then open store page
            await args.OpenStorePageAsync();

            // wait for app window to be re-activated
            var tcs = new TaskCompletionSource<object>();
            WindowActivatedEventHandler handler = (s, e) =>
            {
                if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
                {
                    tcs.TrySetResult(null);
                }
            };
            Window.Current.Activated += handler;
            await tcs.Task;
            Window.Current.Activated -= handler;

            // now refresh codec checker, so next file might use HW acceleration (if codec was really installed)
            await CodecChecker.RefreshAsync();
        }

        async void OnLiveChanged()
        {
            SetSize(MediaSource.CurrentVideoStream);
            var Config = new MediaSourceConfig()
            {
                //VideoDecoderMode = VideoDecoderMode.ForceFFmpegSoftwareDecoder
            };
            Config.ReadAheadBufferEnabled = true;
            
            // Optionally, configure buffer size (max duration and byte size)
            Config.ReadAheadBufferDuration = TimeSpan.FromSeconds(30);
            Config.ReadAheadBufferSize = 50 * 1024 * 1024;
            
            Config.FFmpegOptions.Add("stimeout", 1000000);
            Config.FFmpegOptions.Add("timeout", 1000000);
            Config.FFmpegOptions.Add("reconnect", 1);
            Config.FFmpegOptions.Add("reconnect_streamed", 1);
            Config.FFmpegOptions.Add("reconnect_on_network_error", 1);

            MediaSource = await FFmpegMediaSource.CreateFromUriAsync(Live.RtmpPlaybackUrl, Config);
            MediaStreamSource = MediaSource.GetMediaStreamSource();
            mediaPlayerElement.SetMediaStreamSource(MediaStreamSource);
        }

        void SetSize(VideoStreamInfo props)
        {
            var nav = App.Container.GetService<NavigationService>();
            if(nav.Content is FrameworkElement parentFrame)
            {
                var size = ControlSizeHelper.CalculateSizeInBox(props.PixelWidth, props.PixelHeight, parentFrame.ActualHeight, parentFrame.ActualWidth);
                PageHeight = size.Height;
                PageWidth = size.Width;
                mediaPlayerElement.Height = PageHeight;
                mediaPlayerElement.Width = PageWidth;
            }
        }
    }
}
