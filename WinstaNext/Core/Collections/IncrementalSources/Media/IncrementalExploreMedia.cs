using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WinstaNext.Core.Collections.IncrementalSources.Media
{
    public class IncrementalExploreMedia : IIncrementalSource<InstaMedia>
    {
        PaginationParameters Pagination { get; set; }

        public IncrementalExploreMedia()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool nomoreitems = false;
        public async Task<IEnumerable<InstaMedia>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (nomoreitems) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.FeedProcessor.GetTopicalExploreFeedAsync(Pagination, cancellationToken);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                if (!result.Value.MoreAvailable) nomoreitems = true;
                return result.Value.Medias;
            }
        }

    }
}
