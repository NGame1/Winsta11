using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinstaNext.Core.Collections.IncrementalSources.Search
{
    [AddINotifyPropertyChangedInterface]
    public class IncrementalHashtagsSearch : IIncrementalSource<InstaHashtag>
    {
        [OnChangedMethod(nameof(OnSearchQuerryChanged))]
        public string SearchQuerry { get; set; } = String.Empty;

        PaginationParameters pagination { get; set; }

        public IncrementalHashtagsSearch()
        {
            pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaHashtag>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.HashtagProcessor.SearchHashtagAsync(SearchQuerry);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                HasMoreAvailable = false;
                return result.Value;
            }
        }
        void OnSearchQuerryChanged()
        {
            pagination = PaginationParameters.MaxPagesToLoad(1);
            HasMoreAvailable = true;
        }
    }

}
