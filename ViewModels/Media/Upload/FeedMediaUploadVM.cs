using FFmpegInteropX;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Editing;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinstaCore;
using WinstaCore.Converters.FileConverters;
using WinstaCore.Helpers;
using WinstaCore.Helpers.FFMpegHelpers;
using WinstaCore.Interfaces.Views.Medias.Upload;
#nullable enable

namespace ViewModels.Media.Upload
{
    public class FeedMediaUploadVM : BaseViewModel
    {
        StorageFile? File { get; set; }
        FFmpegMediaSource? FFMediaSource { get; set; }
        MediaStreamSource? StreamSource { get; set; }

        public MediaPlaybackItem? VideoMedia { get; private set; }
        IVideoMediaRangeSlider? VideoMediaRangeSlider { get; set; }

#if !WINDOWS_UWP15063
        public AsyncRelayCommand<ImageCropper> CropCommand { get; set; }
        public AsyncRelayCommand<ImageCropper> CropDoneCommand { get; set; }
#else
        public AsyncRelayCommand<ImageCropper.UWP.ImageCropper> CropCommand { get; set; }
        public AsyncRelayCommand<ImageCropper.UWP.ImageCropper> CropDoneCommand { get; set; }
#endif

        public AsyncRelayCommand ExportVideoCommand { get; set; }
        public RelayCommand CancelTranscodeCommand { get; set; }
        public RelayCommand PlayCommand { get; set; }
        public RelayCommand PauseCommand { get; set; }

        public Visibility PrimarybarVisibilithy { get; set; } = Visibility.Visible;
        public Visibility CropbarVisibilithy { get; set; } = Visibility.Collapsed;

        public int Progress { get; set; } = 0;

        Rect Rect { get; set; } = new(0, 0, 1, 1);

        CancellationTokenSource? TranscodeCancellationToken { get; set; } = null;

        public FeedMediaUploadVM()
        {
            CancelTranscodeCommand = new(CancelTranscode);
            ExportVideoCommand = new(ExportVideoAsync);
            CropDoneCommand = new(CropDoneAsync);
            CropCommand = new(CropAsync);
            PauseCommand = new(Pause);
            PlayCommand = new(Play);
        }

        void Pause()
        {
            if (VideoMedia == null) return;
            VideoMediaRangeSlider?.MediaElement?.Pause();
        }

        async void Play()
        {
            if (VideoMedia == null)
                await CreateMediaPlaybackItemAsync(Rect);
            VideoMediaRangeSlider?.MediaElement?.Play();
        }

        void CancelTranscode()
        {
            TranscodeCancellationToken?.Cancel();
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

#if !WINDOWS_UWP15063
        async Task CropDoneAsync(ImageCropper? obj)
#else
        async Task CropDoneAsync(ImageCropper.UWP.ImageCropper? obj)
#endif
        {
            if (obj == null || VideoMediaRangeSlider == null) return;
            SetVisibility(nameof(PrimarybarVisibilithy));

#if !WINDOWS_UWP15063
            Rect = obj.CroppedRegion;
            obj.Source = null;
#else
            var croprect = obj.GetType().GetRuntimeFields().FirstOrDefault(x=>x.Name == "_currentCroppedRect");
            if (croprect == null)
            {
                await new MessageDialog(nameof(croprect)).ShowAsync();
                return;
            }
            var value = croprect.GetValue(obj);
            Rect = (Rect)value;
            obj.SourceImage = null;
#endif
            VideoMediaRangeSlider.MediaElement.Pause();
            //var current = VideoMediaRangeSlider.MediaElement.Position;
            await CreateMediaPlaybackItemAsync(Rect);
            //VideoMedia.StartTime.Add(current);
            //VideoMediaRangeSlider.MediaElement.Position = current;
        }

#if !WINDOWS_UWP15063
        async Task CropAsync(ImageCropper? obj)
#else
        async Task CropAsync(ImageCropper.UWP.ImageCropper? obj)
#endif
        {
            if (obj == null) return;
            SetVisibility(nameof(CropbarVisibilithy));
            if (VideoMediaRangeSlider == null) return;
            var thumb = await ExtractVideoThumbnailsAsync(VideoMediaRangeSlider.MediaElement.Position);
            if (thumb == null) return;
            await obj.LoadImageFromFile(thumb);
        }

        async Task ExportVideoAsync()
        {
            //SetVisibility(nameof(ExportingVisibilithy));
            Progress = 0;
            if (File == null || VideoMediaRangeSlider == null) return;
            List<string> encodingPropertiesToRetrieve = new()
            {
                "System.Video.FrameRate"
            };
            IDictionary<string, object> encodingProperties = await File.Properties.RetrievePropertiesAsync(encodingPropertiesToRetrieve);
            uint frameRateX1000 = (uint)encodingProperties["System.Video.FrameRate"];
            VideoMediaRangeSlider.MediaElement.Pause();
            VideoMediaRangeSlider.MediaElement.Source = null; ;
            FFMediaSource = null;
            StreamSource = null;
            VideoMedia = null;

            var start = VideoMediaRangeSlider.RangeMin;
            var startTime = TimeSpan.FromMilliseconds(start);
            var end = VideoMediaRangeSlider.RangeMax;
            var endTime = TimeSpan.FromMilliseconds(end);
            var fileStream = await File.OpenReadAsync();
            FFMediaSource = await FFmpegMediaSource.CreateFromStreamAsync(fileStream);

//#if !WINDOWS_UWP15063
            var rect = Rect;
            Rect = new((rect.X - 1), (rect.Y - 1), (rect.Width - 1), (rect.Height - 1));
            FFMediaSource.CropVideo(Rect);
//#endif
            //FFMediaSource.Scale((int)Rect.Width, (int)Rect.Height);

            StreamSource = FFMediaSource.GetMediaStreamSource();

            var folder = await ApplicationSettingsManager.Instance.GetDownloadsFolderAsync();
            var file = await folder.CreateFileAsync("Test.Mp4", CreationCollisionOption.ReplaceExisting);

            var profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.HD720p);
            profile.Video.Bitrate = (uint)FFMediaSource.CurrentVideoStream.Bitrate;
            profile.Video.Subtype = "H264";
            profile.Video.Height = (uint)Math.Abs(Rect.Height);
            profile.Video.Width = (uint)Math.Abs(Rect.Width);
            profile.Video.FrameRate.Numerator = frameRateX1000 / 1000;
            profile.Video.FrameRate.Denominator = 1;
            profile.Audio.Bitrate = (uint)FFMediaSource.CurrentAudioStream.Bitrate;
            profile.Audio.SampleRate = (uint)FFMediaSource.CurrentAudioStream.SampleRate;
            profile.Audio.BitsPerSample = (uint)FFMediaSource.CurrentAudioStream.BitsPerSample;
            profile.Audio.ChannelCount = (uint)FFMediaSource.CurrentAudioStream.Channels;

            var transcoder = new MediaTranscoder
            {
                AlwaysReencode = true,
                TrimStartTime = startTime,
                TrimStopTime = endTime,
                HardwareAccelerationEnabled = true,
                VideoProcessingAlgorithm = MediaVideoProcessingAlgorithm.MrfCrf444
            };

//#if WINDOWS_UWP15063
//            Rect = new Rect((int)(Rect.X - 1), (int)(Rect.Y - 1), (int)(Rect.Width - 1), (int)(Rect.Height - 1));
//            var videoEffect = new VideoTransformEffectDefinition()
//            {
//                CropRectangle = Rect,
//            };
//            transcoder.AddVideoEffect(videoEffect.ActivatableClassId, true, videoEffect.Properties);
//#endif

            var result = await transcoder
                .PrepareMediaStreamSourceTranscodeAsync(
                StreamSource,
                await file.OpenAsync(FileAccessMode.ReadWrite),
                profile);
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
        }

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
                var func1 = CreateMediaPlaybackItemAsync();
                var func2 = ExtractVideoThumbnailsAsync(videoMediaRangeSlider.AddImageToSlider);
                await func1;
                await func2;
            }
            catch (Exception)
            {
                NavigationService.GoBack();
                throw;
            }
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
            VideoMediaRangeSlider.MediaElement.SetPlaybackSource(VideoMedia);
            VideoMediaRangeSlider.MediaElement.Play();
        }

        //async void VideoMediaRangeSlider_ValueChanged(object sender, EventArgs e)
        //{
        //    await CreateMediaPlaybackItemAsync(TimeSpan.FromMilliseconds(VideoMediaRangeSlider.RangeMin), TimeSpan.FromMilliseconds(VideoMediaRangeSlider.RangeMax));
        //    StartMediaPlayback();
        //}

        async Task CreateMediaPlaybackItemAsync()
        {
            if (File == null) return;
            var fileStream = await File.OpenReadAsync();
            FFMediaSource = await FFmpegMediaSource.CreateFromStreamAsync(fileStream);
            if (NavigationService.Content is not FrameworkElement) return;
            var cvs = FFMediaSource.CurrentVideoStream;
            Rect = new(0, 0, cvs.PixelWidth, cvs.PixelHeight);
            VideoMedia = FFMediaSource.CreateMediaPlaybackItem();
            StartMediaPlayback();
        }

        async Task CreateMediaPlaybackItemAsync(Rect rect)
        {
            if (File == null) return;
            var fileStream = await File.OpenReadAsync();
            FFMediaSource = await FFmpegMediaSource.CreateFromStreamAsync(fileStream);
            if (NavigationService.Content is not FrameworkElement) return;
            FFMediaSource.CropVideo(rect);
            VideoMedia = FFMediaSource.CreateMediaPlaybackItem();
            StartMediaPlayback();
        }

        public async Task CreateMediaPlaybackItemAsync(TimeSpan start, TimeSpan end)
        {
            if (File == null) return;
            var fileStream = await File.OpenReadAsync();
            FFMediaSource = await FFmpegMediaSource.CreateFromStreamAsync(fileStream);
            VideoMedia = FFMediaSource.CreateMediaPlaybackItem(start, end - start);
            StartMediaPlayback();
        }

        async Task<StorageFile?> ExtractVideoThumbnailsAsync(TimeSpan captureTime)
        {
            if (File == null) return null;
            var fileStream = await File.OpenReadAsync();
            var frameGrabber = await FrameGrabber.CreateFromStreamAsync(fileStream);
            if (NavigationService.Content is not FrameworkElement) return null;
            var inmemoryStream = new InMemoryRandomAccessStream();
            var frame = await frameGrabber.ExtractVideoFrameAsync(captureTime);
            await frame.EncodeAsJpegAsync(inmemoryStream);
            var bytes = await FileConverter.ToBytesAsync(inmemoryStream);
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("Crop.jpg", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteBytesAsync(file, bytes);
            return file;
        }

        async Task ExtractVideoThumbnailsAsync(Action<ImageSource> AddImageToSlider)
        {
            if (File == null || VideoMediaRangeSlider == null) return;
            var fileStream = await File.OpenReadAsync();
            var frameGrabber = await FrameGrabber.CreateFromStreamAsync(fileStream);
            var duration = frameGrabber.Duration;
            VideoMediaRangeSlider.Minimum = 0;
            VideoMediaRangeSlider.RangeMin = 0;
            VideoMediaRangeSlider.Maximum = duration.TotalMilliseconds;
            VideoMediaRangeSlider.RangeMax = duration.TotalMilliseconds;
            if (NavigationService.Content is not FrameworkElement element) return;
            var boundSize = ControlSizeHelper.CalculateSizeInBox(1080, 1920,
                            element.ActualHeight, element.ActualWidth);
            var size = ControlSizeHelper.CalculateSizeInBox(
                       frameGrabber.CurrentVideoStream.PixelWidth,
                       frameGrabber.CurrentVideoStream.PixelHeight,
                       boundSize.Height, boundSize.Width);
            var thumbnailSize = ControlSizeHelper.CalculateSizeInBox(
                                frameGrabber.CurrentVideoStream.PixelHeight,
                                frameGrabber.CurrentVideoStream.PixelWidth,
                                85, 85);
            //VideoMediaRangeSlider.MediaElement.Height = size.Height;
            VideoMediaRangeSlider.MediaElement.Width = size.Width;
            var imagesCount = (element.ActualWidth - 40) / thumbnailSize.Width;
            imagesCount += (duration.TotalMilliseconds % imagesCount) == 0 ? 0 : 1;
            int skipTime = Convert.ToInt32(duration.TotalMilliseconds / imagesCount);
            for (int i = 0; i < duration.TotalMilliseconds; i += skipTime)
            {
                var inmemoryStream = new InMemoryRandomAccessStream();
                var frame = await frameGrabber.ExtractVideoFrameAsync(TimeSpan.FromMilliseconds(i));
                await frame.EncodeAsJpegAsync(inmemoryStream);
                //We should add all frames to VideoMediaRangeSlider
                var bmp = new BitmapImage { };
                await bmp.SetSourceAsync(inmemoryStream);
                AddImageToSlider?.Invoke(bmp);
            }
        }
    }
}
