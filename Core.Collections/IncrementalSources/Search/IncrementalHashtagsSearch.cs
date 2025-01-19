using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Search
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
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.HashtagProcessor.SearchHashtagAsync(SearchQuerry, pagination);
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
