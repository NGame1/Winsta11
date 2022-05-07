using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace WinstaNext.Helpers.DownloadUploadHelper
{
    public static class DownloadHelper
    {
        public static void Download(InstaMedia Media)
        {
            switch (Media.MediaType)
            {
                case InstaMediaType.Image:
                    DownloadImage(Media.Images[0].Uri, Media);
                    break;
                case InstaMediaType.Video:
                    DownloadVideo(Media.Videos[0].Uri, Media);
                    break;
                case InstaMediaType.Carousel:
                    for (int i = 0; i < Media.Carousel.Count; i++)
                    {
                        var ci = Media.Carousel.ElementAt(i);
                        if (ci.MediaType == InstaMediaType.Image)
                            DownloadImage(ci.Images[0].Uri, Media);
                        else
                            DownloadVideo(ci.Videos[0].Uri, Media);
                    }
                    break;
            }
        }

        static async void DownloadImage(string imageUri, InstaMedia Media)
        {
            var bgdl = App.Container.GetService<BackgroundDownloader>();
            var WinstaFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Winsta", CreationCollisionOption.OpenIfExists);
            var userDownloads = await WinstaFolder.CreateFolderAsync(Media.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"{Media.InstaIdentifier}.jpg", CreationCollisionOption.GenerateUniqueName);
            if (Uri.TryCreate(imageUri, UriKind.RelativeOrAbsolute, out var uri))
            {
                bgdl.TransferGroup = BackgroundTransferGroup.CreateGroup(Media.InstaIdentifier);
                bgdl.SuccessToastNotification = await NotifyHelper.CreateNotifyEmptyAsync(LanguageManager.Instance.Instagram.SuccessToastNotification, desfile.Path, Media.User.ProfilePicture);
                bgdl.FailureToastNotification = await NotifyHelper.CreateNotifyEmptyAsync(LanguageManager.Instance.Instagram.FailureToastNotification, desfile.Path, Media.User.ProfilePicture);

                var dl = bgdl.CreateDownload(uri, desfile);
                dl.StartAsync().AsTask();
            }
        }

        static async void DownloadVideo(string videoUri, InstaMedia Media)
        {
            var bgdl = App.Container.GetService<BackgroundDownloader>();
            var WinstaFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Winsta", CreationCollisionOption.OpenIfExists);
            var userDownloads = await WinstaFolder.CreateFolderAsync(Media.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"{Media.InstaIdentifier}.mp4", CreationCollisionOption.GenerateUniqueName);
            if (Uri.TryCreate(videoUri, UriKind.RelativeOrAbsolute, out var uri))
            {
                bgdl.TransferGroup = BackgroundTransferGroup.CreateGroup(Media.InstaIdentifier);
                bgdl.SuccessToastNotification = await NotifyHelper.CreateNotifyEmptyAsync(LanguageManager.Instance.Instagram.SuccessToastNotification, desfile.Path, Media.User.ProfilePicture);
                bgdl.FailureToastNotification = await NotifyHelper.CreateNotifyEmptyAsync(LanguageManager.Instance.Instagram.FailureToastNotification, desfile.Path, Media.User.ProfilePicture);
                var dl = bgdl.CreateDownload(uri, desfile);

                dl.StartAsync().AsTask();
            }
        }
    }
}
