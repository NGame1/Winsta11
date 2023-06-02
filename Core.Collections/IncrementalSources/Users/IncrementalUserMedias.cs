using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Users
{
    public class IncrementalUserMedias : IIncrementalSource<InstaMedia>
    {
        PaginationParameters Pagination { get; }
        public long UserId { get; private set; } = -1;

        public IncrementalUserMedias(long userId)
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            UserId = userId;
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaMedia>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.GetUserMediaByIdAsync(UserId, Pagination, cancellationToken, false);
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
