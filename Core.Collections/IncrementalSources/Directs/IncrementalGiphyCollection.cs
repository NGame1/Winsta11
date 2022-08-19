using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using Microsoft.Toolkit.Collections;
using PropertyChanged;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Directs
{
    public class IncrementalDirectGiphyCollection : IIncrementalSource<GiphyItem>
    {
        PaginationParameters Pagination { get; set; }

        [OnChangedMethod(nameof(OnSearchQueryChanged))]
        public string SearchQuery { get; set; } = string.Empty;

        public IncrementalDirectGiphyCollection()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool nomoreitems = false;
        public async Task<IEnumerable<GiphyItem>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (nomoreitems) return null;
            using (IInstaApi Api = AppCore.Container?.GetService<IInstaApi>())
            {
                if(SearchQuery == string.Empty)
                {
                    var result = await Api.GetGiphyTrendingAsync(InstaGiphyRequestType.Direct);
                    if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                        throw result.Info.Exception;
                    nomoreitems = false;
                    return result.Value.Items;
                }
                else
                {
                    var result = await Api.SearchGiphyAsync(SearchQuery, InstaGiphyRequestType.Direct);
                    if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                        throw result.Info.Exception;
                    nomoreitems = false;
                    return result.Value.Items;
                }
            }
        }

        void OnSearchQueryChanged()
        {
            this.Pagination = PaginationParameters.MaxPagesToLoad(1);
            nomoreitems = false;
        }
    }
}
