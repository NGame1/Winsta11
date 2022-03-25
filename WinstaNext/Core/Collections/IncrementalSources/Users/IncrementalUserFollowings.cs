using InstagramApiSharp;
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

namespace WinstaNext.Core.Collections.IncrementalSources.Users
{
    public class IncrementalUserFollowings : IIncrementalSource<InstaUserShort>
    {
        PaginationParameters Pagination { get; }
        public long UserId { get; private set; } = -1;

        public IncrementalUserFollowings(long userId)
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            UserId = userId;
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaUserShort>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.GetUserFollowingByIdAsync(UserId, Pagination, cancellationToken);
                if (!result.Succeeded)
                {
                    if (result.Info.Exception != null && result.Info.Exception is not TaskCanceledException)
                        throw result.Info.Exception;
                    else if (result.Info.Exception != null)
                        throw result.Info.Exception;
                    else return null;
                }
                HasMoreAvailable = !string.IsNullOrEmpty(result.Value.NextMaxId);
                return result.Value;
            }
        }
    }
}
