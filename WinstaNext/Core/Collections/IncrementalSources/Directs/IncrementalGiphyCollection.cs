using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using PropertyChanged;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WinstaNext.Core.Collections.IncrementalSources.Directs
{
    internal class IncrementalDirectGiphyCollection : IIncrementalSource<GiphyItem>
    {
        PaginationParameters Pagination { get; }

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
            using (IInstaApi Api = App.Container?.GetService<IInstaApi>())
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
            nomoreitems = false;
        }
    }
}
