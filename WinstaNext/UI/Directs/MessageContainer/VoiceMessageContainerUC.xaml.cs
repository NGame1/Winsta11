using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Abstractions.Direct.Models;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs.MessageContainer
{
    public sealed partial class VoiceMessageContainerUC : MessageContainerUC
    {
        RelayCommand PlayPauseCommand { get; set; }
        MediaPlayer MediaPlayer { get; }
        public VoiceMessageContainerUC()
        {
            MediaPlayer = App.Container.GetService<MediaPlayer>();
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
