using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinstaNext.Abstractions.Media;

namespace WinstaNext.Core.Collections.IncrementalSources.Media
{
    public class IncrementalMediaLikers : IIncrementalSource<WinstaMediaLikerUser>
    {
        public string MediaId { get; }

        public IncrementalMediaLikers(string mediaId)
        {
            MediaId = mediaId;
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<WinstaMediaLikerUser>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.MediaProcessor.GetMediaLikersAsync(MediaId);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;

                var userIds = result.Value.Select(x => x.Pk).ToArray();
                var friendshipstatusresult = await Api.UserProcessor.GetFriendshipStatusesAsync(userIds);
                if (!friendshipstatusresult.Succeeded) throw friendshipstatusresult.Info.Exception;

                friendshipstatusresult.Value.OrderBy(x => userIds);

                HasMoreAvailable = false;
                var count1 = result.Value.Count;
                var count2 = friendshipstatusresult.Value.Count;

                List<WinstaMediaLikerUser> users = new();

                for (int i = 0; i < friendshipstatusresult.Value.Count; i++)
                {
                    users.Add(new WinstaMediaLikerUser(result.Value.ElementAt(i), friendshipstatusresult.Value.ElementAt(i)));
                }

                return users;
            }
        }
    }
}
