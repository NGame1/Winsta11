using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Resources;
using System;
using System.Linq;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using WinstaCore;
#nullable enable

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

        public static void Download(InstaStoryItem Story)
        {
            switch (Story.MediaType)
            {
                case InstaMediaType.Image:
                    DownloadImage(Story.Images[0].Uri, Story);
                    break;
                case InstaMediaType.Video:
                    DownloadVideo(Story.Videos[0].Uri, Story);
                    break;
            }
        }

        static async void DownloadImage(string imageUri, InstaMedia Media)
        {
            var WinstaFolder = await ApplicationSettingsManager.Instance.GetDownloadsFolderAsync();
            var userDownloads = await WinstaFolder.CreateFolderAsync(Media.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"{Media.InstaIdentifier}.jpg", CreationCollisionOption.GenerateUniqueName);
            Download(desfile, imageUri, Media.InstaIdentifier, Media.User.ProfilePicture);
        }

        static async void DownloadImage(string imageUri, InstaStoryItem Story)
        {
            var WinstaFolder = await ApplicationSettingsManager.Instance.GetDownloadsFolderAsync();
            var userDownloads = await WinstaFolder.CreateFolderAsync(Story.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"Story_{Story.Id}.jpg", CreationCollisionOption.GenerateUniqueName);
            Download(desfile, imageUri, Story.Id, Story.User.ProfilePicture);
        }

        static async void DownloadVideo(string videoUri, InstaMedia Media)
        {
            var WinstaFolder = await ApplicationSettingsManager.Instance.GetDownloadsFolderAsync();
            var userDownloads = await WinstaFolder.CreateFolderAsync(Media.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"{Media.InstaIdentifier}.mp4", CreationCollisionOption.GenerateUniqueName);
            Download(desfile, videoUri, Media.InstaIdentifier, Media.User.ProfilePicture);
        }

        static async void DownloadVideo(string videoUri, InstaStoryItem Story)
        {
            var WinstaFolder = await ApplicationSettingsManager.Instance.GetDownloadsFolderAsync();
            var userDownloads = await WinstaFolder.CreateFolderAsync(Story.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"Story_{Story.Id}.mp4", CreationCollisionOption.GenerateUniqueName);
            Download(desfile, videoUri, Story.Id, Story.User.ProfilePicture);
        }

        static async void Download(StorageFile destinationFile, string downloadUri, string GroupIdentifier, string? profilePicture)
        {
            if(profilePicture == null) profilePicture = string.Empty;
            var bgdl = AppCore.Container.GetService<BackgroundDownloader>();
            if(bgdl == null) return;
            if (Uri.TryCreate(downloadUri, UriKind.RelativeOrAbsolute, out var uri))
            {
                bgdl.TransferGroup = BackgroundTransferGroup.CreateGroup(GroupIdentifier);
                bgdl.SuccessToastNotification = await NotifyHelper.CreateNotifyEmptyAsync(LanguageManager.Instance.Instagram.SuccessToastNotification, destinationFile.Path, profilePicture);
                bgdl.FailureToastNotification = await NotifyHelper.CreateNotifyEmptyAsync(LanguageManager.Instance.Instagram.FailureToastNotification, destinationFile.Path, profilePicture);

                var dl = bgdl.CreateDownload(uri, destinationFile);
                dl.StartAsync().AsTask();
            }
        }
    }
}
