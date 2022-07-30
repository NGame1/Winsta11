using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using Windows.Media.Core;
using Windows.Media.Playback;
using WinstaCore;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Directs.MessageContainer
{
    public sealed partial class VoiceMessageContainerUC : MessageContainerUC
    {
        RelayCommand PlayPauseCommand { get; set; }
        MediaPlayer MediaPlayer { get; }
        public VoiceMessageContainerUC()
        {
            MediaPlayer = AppCore.Container.GetService<MediaPlayer>();
            PlayPauseCommand = new RelayCommand(PlayPause);
            this.InitializeComponent();
        }

        private void PlayPause()
        {
            var uri = new Uri(DirectItem.VoiceMedia.Media.Audio.AudioSource, UriKind.RelativeOrAbsolute);
            MediaPlayer.Source = MediaSource.CreateFromUri(uri);
            MediaPlayer.Play();
        }
    }
}
