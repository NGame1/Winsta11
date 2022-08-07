using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp;
using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Activities
{
    public class IncrementalPendingFriendRequests : IIncrementalSource<InstaUserShortFriendship>
    {
        PaginationParameters Pagination { get; set; }

        public IncrementalPendingFriendRequests()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaUserShortFriendship>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.GetPendingFriendRequestsAsync(Pagination, cancellationToken);

                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;

                HasMoreAvailable = !string.IsNullOrEmpty(result.Value.NextMaxId);

                return result.Value.Users;
            }
        }

        public void RequestRefresh()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }
    }
}
