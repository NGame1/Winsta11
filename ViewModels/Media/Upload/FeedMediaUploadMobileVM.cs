#if WINDOWS_UWP15063
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinstaCore;
using WinstaCore.Converters.FileConverters;
using WinstaCore.Helpers;
using WinstaCore.Interfaces.Views.Medias.Upload;
#nullable enable

namespace ViewModels.Media.Upload
{
    public class FeedMediaUploadMobileVM : BaseViewModel
    {
        StorageFile? File { get; set; }
        MediaComposition? Composition { get; set; }
        MediaStreamSource? StreamSource { get; set; }

        IVideoMediaRangeSlider? VideoMediaRangeSlider { get; set; }

        public AsyncRelayCommand<ImageCropper.UWP.ImageCropper> CropCommand { get; set; }
        public AsyncRelayCommand<ImageCropper.UWP.ImageCropper> CropDoneCommand { get; set; }

        public AsyncRelayCommand ExportVideoCommand { get; set; }
        public RelayCommand CancelTranscodeCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }

        public Visibility PrimarybarVisibilithy { get; set; } = Visibility.Visible;
        public Visibility CropbarVisibilithy { get; set; } = Visibility.Collapsed;

        public int Progress { get; set; } = 0;

        Rect Rect { get; set; } = new(0, 0, 1, 1);

        CancellationTokenSource? TranscodeCancellationToken { get; set; } = null;

        public FeedMediaUploadMobileVM()
        {
            CancelTranscodeCommand = new(CancelTranscode);
            ExportVideoCommand = new(ExportVideoAsync);
            CropDoneCommand = new(CropDoneAsync);
            CropCommand = new(CropAsync);
            PauseCommand = new(Pause);
            PlayCommand = new(Play);
        }

        void SetVisibility(string propertyName)
        {
            var Properties = this.GetType().GetProperties();
            foreach (var property in Properties)
            {
                if (property.PropertyType != typeof(Visibility)) continue;
                if (property.Name == propertyName)
                    property.SetValue(this, Visibility.Visible);
                else property.SetValue(this, Visibility.Collapsed);
            }
        }

        #region Crop
        async Task CropDoneAsync(ImageCropper.UWP.ImageCropper? obj)
        {
            if (obj == null || VideoMediaRangeSlider == null) return;
            SetVisibility(nameof(PrimarybarVisibilithy));

            var croprect = obj.GetType().GetRuntimeFields().FirstOrDefault(x => x.Name == "_currentCroppedRect");
            if (croprect == null)
            {
                await new MessageDialog(nameof(croprect)).ShowAsync();
                return;
            }
            var value = croprect.GetValue(obj);
            Rect = (Rect)value;
            obj.Visibility = Visibility.Collapsed;

            VideoMediaRangeSlider.MediaElement.Pause();
            //var current = VideoMediaRangeSlider.MediaElement.Position;
            await CreateMediaPlaybackItemAsync(Rect);
            //VideoMedia.StartTime.Add(current);
            //VideoMediaRangeSlider.MediaElement.Position = current;
        }

        async Task CropAsync(ImageCropper.UWP.ImageCropper? obj)
        {
            if (obj == null) return;
            if (obj.Visibility == Visibility.Collapsed)
                obj.Visibility = Visibility.Visible;
            SetVisibility(nameof(CropbarVisibilithy));
            if (VideoMediaRangeSlider == null) return;
            var thumb = await ExtractVideoThumbnailsAsync(VideoMediaRangeSlider.MediaElement.Position);
            if (thumb == null) return;
            await obj.LoadImageFromFile(thumb);
        }
        #endregion

        #region Export Video 
        void CancelTranscode()
        {
            TranscodeCancellationToken?.Cancel();
        }
        async Task ShowMessageAsync(string message) => await Task.Delay(500);
        //async Task ShowMessageAsync(string message) => await new MessageDialog(message).ShowAsync();
        async Task ExportVideoAsync()
        {
            //SetVisibility(nameof(ExportingVisibilithy));
            Progress = 0;
            if (File == null || VideoMediaRangeSlider == null) return;
            await ShowMessageAsync("ExportVideoAsync");
            List<string> encodingPropertiesToRetrieve = new()
            {
                "System.Video.FrameRate"
            };
            IDictionary<string, object> encodingProperties = await File.Properties.RetrievePropertiesAsync(encodingPropertiesToRetrieve);
            uint frameRateX1000 = (uint)encodingProperties["System.Video.FrameRate"];
            VideoMediaRangeSlider.MediaElement.Pause();
            VideoMediaRangeSlider.MediaElement.Source = null; ;
            Composition = null;
            StreamSource = null;

            var start = VideoMediaRangeSlider.RangeMin;
            var startTime = TimeSpan.FromMilliseconds(start);
            var end = VideoMediaRangeSlider.RangeMax;
            var endTime = TimeSpan.FromMilliseconds(end);
            var fileStream = await File.OpenReadAsync();
            Composition = new MediaComposition();
            var clip = await MediaClip.CreateFromFileAsync(File);
            var rect = Rect;
            Rect = new((int)(rect.X), (int)(rect.Y), (int)(rect.Width - 1), (int)(rect.Height - 1));
            var videoEffect = new VideoTransformEffectDefinition()
            {
                CropRectangle = Rect,
            };
            await ShowMessageAsync($"MediaClip created for file. start: {startTime}, end: {endTime}, rect: {Rect.X}, {Rect.Y}, {Rect.Width}, {Rect.Height}");
            var vidProps = await File.Properties.GetVideoPropertiesAsync();
            //var audioProps = await File.Properties.GetMusicPropertiesAsync();
            AudioEncodingProperties? audioProps = null;
            if (clip.EmbeddedAudioTracks.Any())
            {
                audioProps = clip.EmbeddedAudioTracks.FirstOrDefault().GetAudioEncodingProperties();
            }
            var folder = await ApplicationSettingsManager.Instance.GetDownloadsFolderAsync();
            var file = await folder.CreateFileAsync("Test.Mp4", CreationCollisionOption.ReplaceExisting);

            var profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD720p);
            profile.Video.Bitrate = (uint)vidProps.Bitrate;
            profile.Video.Subtype = "H264";
            profile.Video.Height = (uint)Math.Abs(Rect.Height);
            profile.Video.Width = (uint)Math.Abs(Rect.Width);
            profile.Video.FrameRate.Numerator = frameRateX1000 / 1000;
            profile.Video.FrameRate.Denominator = 1;
            if (audioProps != null)
            {
                profile.Audio.Bitrate = audioProps.Bitrate;
                profile.Audio.SampleRate = audioProps.SampleRate;
                profile.Audio.BitsPerSample = audioProps.BitsPerSample;
                profile.Audio.ChannelCount = audioProps.ChannelCount;
            }
            Composition.Clips.Add(clip);
            await ShowMessageAsync("Media Clip added to the Composition.");
            StreamSource = Composition.GenerateMediaStreamSource();
            var transcoder = new MediaTranscoder
            {
                TrimStartTime = startTime,
                TrimStopTime = endTime,
            };
            await ShowMessageAsync("Transcoder Initialized.");
            transcoder.AddVideoEffect(videoEffect.ActivatableClassId, true, videoEffect.Properties);
            var result = await transcoder
                .PrepareMediaStreamSourceTranscodeAsync(
                StreamSource,
                await file.OpenAsync(FileAccessMode.ReadWrite),
                profile);
            await ShowMessageAsync("result.CanTranscode: " + result.CanTranscode);
            if (result.CanTranscode)
            {
                try
                {
                    TranscodeCancellationToken = new CancellationTokenSource();
                    IProgress<double> progress = new Progress<double>((newValue) =>
                    {
                        UIContext.Post((val) =>
                        {
                            Progress = Convert.ToInt16(newValue);
                            if (newValue == 100)
                            {
                                //SetVisibility(nameof(PrimarybarVisibilithy));
                                var uploader = AppCore.Container.GetService<IFeedUploaderView>();
                                NavigationService.Navigate(uploader, file);
                                //await new MessageDialog("Transcode Complete.").ShowAsync();
                            }
                        }, null);
                        Debug.WriteLine(newValue);
                    });
                    var transcodeProgress = result.TranscodeAsync().AsTask(TranscodeCancellationToken.Token, progress);
                    await transcodeProgress;
                }
                catch (Exception ex)
                {
                    var exce = ex;
                }
                finally
                {
                    //await Launcher.LaunchFolderAsync(await file.GetParentAsync());
                }
            }
            else
            {
                await ShowMessageAsync(result.FailureReason.ToString());
            }
        }
        #endregion

        public async void BeginLoading(StorageFile file, IVideoMediaRangeSlider videoMediaRangeSlider)
        {
            IsLoading = true;
            try
            {
                if (file == null || videoMediaRangeSlider == null) return;
                File = file;
                VideoMediaRangeSlider = videoMediaRangeSlider;
                //videoMediaRangeSlider.MinimumValueChanged += VideoMediaRangeSlider_ValueChanged;
                //videoMediaRangeSlider.MaximumValueChanged += VideoMediaRangeSlider_ValueChanged;

                var vidprops = await File.Properties.GetVideoPropertiesAsync();
                Rect = new(0, 0, vidprops.Width, vidprops.Height);

                Composition = new MediaComposition();
                var clip = await MediaClip.CreateFromFileAsync(File);
                await ShowMessageAsync("");
                if (NavigationService.Content is not FrameworkElement) return;
                var rect = Rect;
                Rect = new((int)(rect.X), (int)(rect.Y), (int)(rect.Width - 1), (int)(rect.Height - 1));
                var videoEffect = new VideoTransformEffectDefinition()
                {
                    CropRectangle = Rect,
                };
                Composition.Clips.Add(clip);
                await ShowMessageAsync("");
                StreamSource = Composition.GenerateMediaStreamSource();
                //await CreateMediaPlaybackItemAsync();
                await ShowMessageAsync("");
                await ExtractVideoThumbnailsAsync(videoMediaRangeSlider.AddImageToSlider);
                await ShowMessageAsync("");
                StartMediaPlayback();
            }
            catch (Exception)
            {
                NavigationService.GoBack();
                throw;
            }
        }

        //async void VideoMediaRangeSlider_ValueChanged(object sender, EventArgs e)
        //{
        //    await CreateMediaPlaybackItemAsync(TimeSpan.FromMilliseconds(VideoMediaRangeSlider.RangeMin), TimeSpan.FromMilliseconds(VideoMediaRangeSlider.RangeMax));
        //    StartMediaPlayback();
        //}

        #region Create and control playback media
        async Task CreateMediaPlaybackItemAsync()
        {
            if (File == null) return;
            var vidprops = await File.Properties.GetVideoPropertiesAsync();
            Rect = new(0, 0, vidprops.Width, vidprops.Height);
            await CreateMediaPlaybackItemAsync(Rect);
            //Composition = null;
            //StreamSource = null;
            //Composition = new MediaComposition();
            //var clip = await MediaClip.CreateFromFileAsync(File);
            //if (NavigationService.Content is not FrameworkElement) return;
            //Composition.Clips.Add(clip);
            //StreamSource = Composition.GenerateMediaStreamSource();
        }

        async Task CreateMediaPlaybackItemAsync(Rect rect)
        {
            if (VideoMediaRangeSlider == null || File == null) { await new MessageDialog("Shit!").ShowAsync(); return; }
            await ShowMessageAsync("");
            StopMediaPlayback();
            Composition = null;
            StreamSource = null;
            Composition = new MediaComposition();
            var clip = await MediaClip.CreateFromFileAsync(File);
            await ShowMessageAsync("");
            if (NavigationService.Content is not FrameworkElement) return;
            Rect = new((int)(rect.X), (int)(rect.Y), (int)(rect.Width - 1), (int)(rect.Height - 1));
            var videoEffect = new VideoTransformEffectDefinition()
            {
                CropRectangle = Rect,
            };
            //clip.VideoEffectDefinitions.Add(videoEffect);
            Composition.Clips.Add(clip);
            await ShowMessageAsync("");
            StreamSource = Composition.GenerateMediaStreamSource();
            await ShowMessageAsync("");
            VideoMediaRangeSlider.MediaElement.AddVideoEffect(videoEffect.ActivatableClassId, true, videoEffect.Properties);
            await ShowMessageAsync("");
            StartMediaPlayback();
        }

        public void StopMediaPlayback()
        {
            if (VideoMediaRangeSlider == null) return;
            VideoMediaRangeSlider.MediaElement.Stop();
            VideoMediaRangeSlider.MediaElement.Source = null;
        }

        void StartMediaPlayback()
        {
            if (VideoMediaRangeSlider == null) return;
            VideoMediaRangeSlider.MediaElement.SetMediaStreamSource(StreamSource);
            VideoMediaRangeSlider.MediaElement.Play();
        }

        void Pause()
        {
            if (StreamSource == null) return;
            VideoMediaRangeSlider?.MediaElement?.Pause();
        }

        async void Play()
        {
            if (StreamSource == null)
                await CreateMediaPlaybackItemAsync(Rect);
            VideoMediaRangeSlider?.MediaElement?.Play();
        }

        #endregion

        #region Extract Thumbnail
        async Task<StorageFile?> ExtractVideoThumbnailsAsync(TimeSpan captureTime)
        {
            if (Composition == null) return null;
            using (var thumb = await Composition.GetThumbnailAsync(captureTime, (int)Rect.Width, (int)Rect.Height, VideoFramePrecision.NearestFrame))
            {
                var bytes = await FileConverter.ToBytesAsync(thumb.CloneStream());
                var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Crop.jpg", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteBytesAsync(file, bytes);
                return file;
            }
        }

        async Task ExtractVideoThumbnailsAsync(Action<ImageSource> AddImageToSlider)
        {
            if (Composition == null || File == null || VideoMediaRangeSlider == null) return;
            var duration = Composition.Duration;
            VideoMediaRangeSlider.Minimum = 0;
            VideoMediaRangeSlider.RangeMin = 0;
            VideoMediaRangeSlider.Maximum = duration.TotalMilliseconds;
            VideoMediaRangeSlider.RangeMax = duration.TotalMilliseconds;
            if (NavigationService.Content is not FrameworkElement element) return;
            var boundSize = ControlSizeHelper.CalculateSizeInBox(1080, 1920,
                            element.ActualHeight, element.ActualWidth);
            var size = ControlSizeHelper.CalculateSizeInBox(
                       Rect.Width,
                       Rect.Height,
                       boundSize.Height, boundSize.Width);
            var thumbnailSize = ControlSizeHelper.CalculateSizeInBox(
                                Rect.Height,
                                Rect.Width,
                                85, 85);
            while (true)
            {
                if (VideoMediaRangeSlider.MediaElement == null) await Task.Delay(100);
                else break;
            }
            //VideoMediaRangeSlider.MediaElement.Height = size.Height;
            VideoMediaRangeSlider.MediaElement.Width = size.Width;
            var imagesCount = (element.ActualWidth - 40) / thumbnailSize.Width;
            imagesCount += (duration.TotalMilliseconds % imagesCount) == 0 ? 0 : 1;
            int skipTime = Convert.ToInt32(duration.TotalMilliseconds / imagesCount);
            for (int i = 0; i < duration.TotalMilliseconds; i += skipTime)
            {
                try
                {
                    var thumb = await ExtractVideoThumbnailsAsync(TimeSpan.FromMilliseconds(i));
                    if (thumb == null) continue;
                    var bmp = new BitmapImage { };
                    await bmp.SetSourceAsync(await thumb.OpenAsync(FileAccessMode.Read));
                    AddImageToSlider?.Invoke(bmp);
                }
                catch (Exception)
                {

                }
            }
        }
        #endregion
    }
}
#endif
