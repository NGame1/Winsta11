using FFmpegInteropX;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Converters.FileConverters;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore.Models.ConfigureDelays;
#nullable enable

namespace ViewModels.Media.Upload
{
    public class FeedUploaderVM : BaseViewModel
    {
        public string CaptionText { get; set; } = string.Empty;

        public AsyncRelayCommand UploadCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        CancellationTokenSource? _cts = null;
        StorageFile? File { get; set; }

        public FeedUploaderVM()
        {
            UploadCommand = new(UploadAsync);
            CancelCommand = new(Cancel);
        }

        void Cancel() => _cts?.Cancel();

        public async Task UploadAsync()
        {
            IsLoading = true;
            try
            {
                if (File == null) throw new ArgumentNullException(nameof(File));
                using var Api = AppCore.Container.GetService<IInstaApi>();
                if (Api == null) throw new ArgumentNullException(nameof(Api));

                using var ms = new InMemoryRandomAccessStream();
                var props = await File.Properties.GetVideoPropertiesAsync();
                var thumb = await FrameGrabber.CreateFromStreamAsync(await File.OpenAsync(FileAccessMode.Read));
                var thumbnail = await thumb.ExtractNextVideoFrameAsync();
                await thumbnail.EncodeAsJpegAsync(ms);

                Api.SetConfigureMediaDelay(new VideoConfigureMediaDelay());

                _cts = new();
                var bytes = await (await File.OpenReadAsync()).ToBytesAsync();
                var task = Api.MediaProcessor.UploadVideoAsync(new()
                {
                    Video = new()
                    {
                        VideoBytes = bytes,
                        Height = (int)props.Height,
                        Width = (int)props.Width,
                    },
                    VideoThumbnail = new()
                    {
                        ImageBytes = await ms.ToBytesAsync(),
                        Height = (int)thumb.DecodePixelHeight,
                        Width = (int)thumb.DecodePixelWidth
                    }
                }, CaptionText).AsAsyncOperation().AsTask(_cts.Token);
                var result = await task;
                if (!result.Succeeded)
                    throw new Exception(result.Info.Message);
                var mediaView = AppCore.Container.GetService<ISingleInstaMediaView>();
                NavigationService.Navigate(mediaView, result.Value);
            }
            catch (Exception)
            {
                throw;
            }
            finally { IsLoading = false; }
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not StorageFile file)
            {
                NavigationService.GoBack();
                return;
            }
            File = file;

            base.OnNavigatedTo(e);
        }
    }
}
