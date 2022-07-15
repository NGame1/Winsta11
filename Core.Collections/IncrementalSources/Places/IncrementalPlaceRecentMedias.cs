using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Places
{
    public class IncrementalPlaceRecentMedias : IIncrementalSource<InstaMedia>
    {
        public long LocationId { get; private set; }
        PaginationParameters Pagination { get; set; }

        public IncrementalPlaceRecentMedias(long locationId)
        {
            LocationId = locationId;
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool MoreAvailable = true;
        public async Task<IEnumerable<InstaMedia>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!MoreAvailable) return null;
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.LocationProcessor.GetRecentLocationFeedsAsync(LocationId,
                                       Pagination,
                                       cancellationToken: cancellationToken);
                
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;

                MoreAvailable = result.Value.MoreAvailable;

                return result.Value.Medias;
            }
        }
    }
}
