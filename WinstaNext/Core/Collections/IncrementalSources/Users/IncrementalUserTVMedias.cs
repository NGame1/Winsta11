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
    internal class IncrementalUserTVMedias : IIncrementalSource<InstaMedia>
    {
        PaginationParameters Pagination { get; }
        public long UserId { get; private set; } = -1;

        public IncrementalUserTVMedias(long userId)
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            UserId = userId;
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaMedia>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.TVProcessor.GetChannelByIdAsync(UserId, PaginationParameters.MaxPagesToLoad(1));
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                HasMoreAvailable = result.Value.HasMoreAvailable;
                return result.Value.Items;
            }
        }
    }
}
