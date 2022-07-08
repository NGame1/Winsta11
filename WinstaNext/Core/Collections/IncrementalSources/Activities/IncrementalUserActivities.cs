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

namespace WinstaNext.Core.Collections.IncrementalSources.Activities
{
    internal class IncrementalUserActivities : IIncrementalSource<InstaRecentActivityFeed>
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
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
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
