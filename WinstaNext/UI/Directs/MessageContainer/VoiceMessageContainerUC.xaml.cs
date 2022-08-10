using InstagramApiSharp.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using WinstaCore;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs.MessageContainer
{
    public sealed partial class VoiceMessageContainerUC : MessageContainerUC
    {
        RelayCommand PlayPauseCommand { get; set; }

        public VoiceMessageContainerUC()
        {
            PlayPauseCommand = new(PlayPause);
            this.InitializeComponent();
        }

        private async void PlayPause()
        {
            var MediaPlayer = AppCore.Container.GetService<MediaPlayer>();
            var dt = DirectItem;
            using var Api = AppCore.Container.GetService<IInstaApi>();
            var voicestream = await Api.HttpClient.GetStreamAsync(dt.VoiceMedia.Media.Audio.AudioSource);
            //var d = dt.VoiceMedia.Media.Audio.WaveformData;
            //if (MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            //{
            //    MediaPlayer.Pause();
            //    BtnPlayPause.Content = "\uE103";
            //    return;
            //}
            //else if (MediaPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            //{
            //    MediaPlayer.Play();
            //    BtnPlayPause.Content = "\uE102";
            //    return;
            //}
            BtnPlayPause.Content = "\uE103";
            var memStream = new MemoryStream();
            await voicestream.CopyToAsync(memStream);
            memStream.Position = 0;
            MediaPlayer.Source = MediaSource.CreateFromStream(memStream.AsRandomAccessStream(), "audio/mpeg");
            //med.Source = MediaSource.CreateFromUri(new Uri(dt.VoiceMedia.Media.Audio.AudioSource, UriKind.RelativeOrAbsolute));
            MediaPlayer.Play();
        }

        private async void WaveGrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Grid MainGrid = (Grid)sender;
            var black = new SolidColorBrush(Colors.Black);
            var dt = DirectItem;
            if (dt == null) return;
            var WaveFormData = dt.VoiceMedia.Media.Audio.WaveformData;
            if (WaveFormData.Length == 0) return;
            await Task.Delay(10);
            var rectwidth = (int)MainGrid.ActualWidth / WaveFormData.Length;
            if (rectwidth == 0) rectwidth = 1;
            for (int i = 0; i < WaveFormData.Length; i++)
            {
                Rectangle rect = new Rectangle() { Fill = black, Height = (WaveFormData[i] * MainGrid.ActualHeight) };
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(rectwidth) });
                Grid.SetColumn(rect, (MainGrid.ColumnDefinitions.Count - 1));
                MainGrid.Children.Add(rect);
            }
        }

        private void BtnPlayPause_Click(object sender, RoutedEventArgs e)
        {
            PlayPause();
        }
    }
}
