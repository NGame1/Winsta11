using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using WinstaCore;

namespace WinstaNext.Helpers.DownloadUploadHelper
{
    public static class UploadHelper
    {
        public static async Task<InstaMedia> UploadImageAsync(InstaImageUpload image, string caption, InstaLocationShort location = null)
        {
            using (var Api = AppCore.Container.GetService<IInstaApi>())
            {
                var res = await Api.MediaProcessor.UploadPhotoAsync(image, caption, location);
                if (!res.Succeeded) throw res.Info.Exception;
                return res.Value;
            }
        }

        public static async Task<InstaMedia> UploadVideoAsync(InstaVideoUpload video, string caption, InstaLocationShort location = null)
        {
            using (var Api = AppCore.Container.GetService<IInstaApi>())
            {
                var res = await Api.MediaProcessor.UploadVideoAsync(video, caption, location);
                if (!res.Succeeded) throw res.Info.Exception;
                return res.Value;
            }
        }

        public static async Task<InstaMedia> UploadAlbumAsync(InstaAlbumUpload[] album, string caption, InstaLocationShort location = null)
        {
            using (var Api = AppCore.Container.GetService<IInstaApi>())
            {
                var res = await Api.MediaProcessor.UploadAlbumAsync(album, caption, location);
                if (!res.Succeeded) throw res.Info.Exception;
                return res.Value;
            }
        }
    }
}
