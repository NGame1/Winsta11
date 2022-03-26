using InstagramApiSharp.Classes.Models;
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class LivePlayerView : Page
    {
        public static readonly DependencyProperty LiveProperty = DependencyProperty.Register(
          "Live",
          typeof(InstaBroadcast),
          typeof(LivePlayerView),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnLiveChanged))]
        public InstaBroadcast Live
        {
            get { return (InstaBroadcast)GetValue(LiveProperty); }
            set { SetValue(LiveProperty, value); }
        }
        AdaptiveMediaSource ams;

        public LivePlayerView()
        {
            this.InitializeComponent();
        }

        async void OnLiveChanged()
        {
            //InitializeAdaptiveMediaSource(new(Live.DashPlaybackUrl, UriKind.RelativeOrAbsolute));
            var ministaPlayer = new MinistaLivePlayback.MinistaPlayer();
            await ministaPlayer.Initialize(new(Live.DashPlaybackUrl, UriKind.RelativeOrAbsolute), mediaPlayerElement);
            //ministaPlayer.GoToLive();
        }

        async private void InitializeAdaptiveMediaSource(System.Uri uri)
        {
            AdaptiveMediaSourceCreationResult result = await AdaptiveMediaSource.CreateFromUriAsync(uri);

            result.MediaSource.AdvancedSettings.AllSegmentsIndependent = false;

            if (result.Status == AdaptiveMediaSourceCreationStatus.Success)
            {
                ams = result.MediaSource;
                //mediaPlayerElement.SetMediaPlayer(new MediaPlayer());
                //mediaPlayerElement.MediaPlayer.Source = MediaSource.CreateFromAdaptiveMediaSource(ams);
                mediaPlayerElement.SetPlaybackSource(MediaSource.CreateFromAdaptiveMediaSource(ams));
                mediaPlayerElement.Play();


                ams.InitialBitrate = ams.AvailableBitrates.Max<uint>();

                //Register for download requests
                //ams.DownloadRequested += DownloadRequested;

                //Register for download failure and completion events
                //ams.DownloadCompleted += DownloadCompleted;
                //ams.DownloadFailed += DownloadFailed;

                //Register for bitrate change events
                //ams.DownloadBitrateChanged += DownloadBitrateChanged;
                //ams.PlaybackBitrateChanged += PlaybackBitrateChanged;

                //Register for diagnostic event
                //ams.Diagnostics.DiagnosticAvailable += DiagnosticAvailable;
            }
            else
            {
                // Handle failure to create the adaptive media source
                //MyLogMessageFunction($"Adaptive source creation failed: {uri} - {result.ExtendedError}");
            }
        }
    }
}
