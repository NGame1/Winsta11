using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Search
{
    [AddINotifyPropertyChangedInterface]
    public class IncrementalPeopleSearch : IIncrementalSource<InstaUser>
    {
        [OnChangedMethod(nameof(OnSearchQuerryChanged))]
        public string SearchQuerry { get; set; } = String.Empty;

        PaginationParameters pagination { get; set; }

        public IncrementalPeopleSearch()
        {
            pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        bool _isLoading = false;
        public async Task<IEnumerable<InstaUser>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            while (_isLoading)
            {
                await Task.Delay(200);
            }
            try
            {
                var currentNextMaxId = pagination.NextMaxId;
                _isLoading = true;
                using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
                var result = await Api.DiscoverProcessor.SearchPeopleAsync(SearchQuerry, pagination,
                             cancellationToken: cancellationToken);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                HasMoreAvailable = result.Value.HasMoreAvailable;
                if (currentNextMaxId == pagination.NextMaxId)
                {
                    return null;
                }
                return result.Value.Users.Distinct();
            }
            finally
            {
                _isLoading = false;
            }
        }

        void OnSearchQuerryChanged()
        {
            pagination = PaginationParameters.MaxPagesToLoad(1);
            HasMoreAvailable = true;
        }
    }

}
