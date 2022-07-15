using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Activities
{
    public class IncrementalUserActivities : IIncrementalSource<InstaRecentActivityFeed>
    {
        PaginationParameters Pagination { get; set; }

        public IncrementalUserActivities()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaRecentActivityFeed>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.GetRecentActivityFeedAsync(Pagination, cancellationToken);

                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;

                HasMoreAvailable = !string.IsNullOrEmpty(result.Value.NextMaxId);

                return result.Value.Items;
            }
        }

        public void RequestRefresh()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }
    }
}
