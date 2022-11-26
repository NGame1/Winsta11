using FFmpegInteropX;
using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using WinstaCore;
using WinstaCore.Helpers;
using WinstaCore.Helpers.FFMpegHelpers;
using WinstaCore.Interfaces.Views.Medias.Upload;

namespace ViewModels.Media.Upload
{
    public class FeedMediaUploadVM : BaseViewModel
    {
        StorageFile File { get; set; }
        FFmpegMediaSource FFMediaSource { get; set; }

        public MediaPlaybackItem VideoMedia { get; private set; }
        IVideoMediaRangeSlider VideoMediaRangeSlider { get; set; }

        public FeedMediaUploadVM()
        {

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

        void StopMediaPlayback()
        {
            VideoMediaRangeSlider.MediaElement.Stop();
            VideoMediaRangeSlider.MediaElement.Source = null;
        }

        void StartMediaPlayback()
        {
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
            var fileStream = await File.OpenReadAsync();
            FFMediaSource = await FFmpegMediaSource.CreateFromStreamAsync(fileStream);
            VideoMedia = FFMediaSource.CreateMediaPlaybackItem();
            StartMediaPlayback();
        }

        async Task CreateMediaPlaybackItemAsync(TimeSpan start, TimeSpan end)
        {
            var fileStream = await File.OpenReadAsync();
            FFMediaSource = await FFmpegMediaSource.CreateFromStreamAsync(fileStream);
            VideoMedia = FFMediaSource.CreateMediaPlaybackItem(start, end - start);
            StartMediaPlayback();
        }

        async Task ExtractVideoThumbnailsAsync(Action<ImageSource> AddImageToSlider)
        {
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
            imagesCount += (duration.TotalSeconds % imagesCount) == 0 ? 0 : 1;
            int skipTime = Convert.ToInt32(duration.TotalSeconds / imagesCount);
            for (int i = 0; i < duration.TotalSeconds; i += skipTime)
            {
                var inmemoryStream = new InMemoryRandomAccessStream();
                var frame = await frameGrabber.ExtractVideoFrameAsync(TimeSpan.FromSeconds(i));
                await frame.EncodeAsJpegAsync(inmemoryStream);
                //We should add all frames to VideoMediaRangeSlider
                var bmp = new BitmapImage { };
                await bmp.SetSourceAsync(inmemoryStream);
                AddImageToSlider?.Invoke(bmp);
            }
        }
    }
}
