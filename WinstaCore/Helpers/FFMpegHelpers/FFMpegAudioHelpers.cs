using FFmpegInteropX;
using Microsoft.Toolkit.Uwp.UI;
using System;
using Windows.System;

namespace WinstaCore.Helpers.FFMpegHelpers
{
    public static class FFMpegAudioHelpers
    {
        /// <summary>
        /// Apply echoing to the input audio.
        /// Echoes are reflected sound and can occur naturally amongst mountains(and sometimes large buildings) when talking or shouting; digital echo effects emulate this behaviour and are often used to help fill out the sound of a single instrument or vocal.The time difference between the original signal and the reflection is the delay, and the loudness of the reflected signal is the decay. Multiple echoes can have different delays and decays.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="inGain">Set input gain of reflected signal. Default is 0.6.</param>
        /// <param name="outGain">Set output gain of reflected signal. Default is 0.3.</param>
        /// <param name="delayMs">Set list of time intervals in milliseconds between original signal and reflections separated by ’|’. Allowed range for each delay is (0 - 90000.0]. Default is 1000.</param>
        /// <param name="decays">Set list of loudness of reflected signals separated by ’|’. Allowed range for each decay is (0 - 1.0]. Default is 0.5.</param>
        public static void Echo(this FFmpegMediaSource source, float inGain = 0.6f, float outGain = 0.3f, int delayMs = 1000, float decays = 0.5f)
        {
            source.SetFFmpegAudioFilters($"aecho=0.8:0.88:{delayMs}:0.4");
        }

        /// <summary>
        /// Simple audio dynamic range compression/expansion filter.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contrast">Set contrast. Default is 33. Allowed range is between 0 and 100.</param>
        public static void Contrast(this FFmpegMediaSource source, int contrast = 33)
        {
            source.SetFFmpegAudioFilters($"acontrast=contrast={contrast}");
        }

        /// <summary>
        /// Adjust audio tempo
        /// </summary>
        /// <param name="source"></param>
        /// <param name="tempo">the audio tempo. If not specified then the filter will assume nominal 1.0 tempo. Tempo must be in the [0.5, 100.0] range.</param>
        /// <exception cref="ArgumentOutOfRangeException">throw ArgumentOutOfRangeException when the tempo value is lower than 0.5 or higher than 100.</exception>
        public static void Tempo(this FFmpegMediaSource source, double tempo = 1.0)
        {
            if (tempo < 0.5 || tempo > 100)
                throw new ArgumentOutOfRangeException(nameof(tempo));
            source.SetFFmpegAudioFilters($"atempo={tempo}");
        }

        /// <summary>
        /// Adjust the input audio volume
        /// </summary>
        /// <param name="source"></param>
        /// <param name="volume">Set audio volume expression. Output values are clipped to the maximum value (default is 1.0)</param>
        public static void Volume(this FFmpegMediaSource source, double volume = 1.0)
        {
            source.SetFFmpegAudioFilters($"volume=volume={volume}");
        }

        /// <summary>
        /// Adjust the input audio volume in Decibels unit
        /// </summary>
        /// <param name="source"></param>
        /// <param name="volumeChange">Set audio volume change in db. Output values are clipped to the maximum value (default is 0.0)</param>
        public static void VolumeByDecibels(this FFmpegMediaSource source, double volumeChange = 0.0)
        {
            source.SetFFmpegAudioFilters($"volume=volume={volumeChange}dB:precision=fixed");
        }
    }
}
