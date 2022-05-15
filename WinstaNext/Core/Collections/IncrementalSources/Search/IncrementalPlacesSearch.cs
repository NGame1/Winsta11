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
    public class IncrementalPlacesSearch : IIncrementalSource<InstaPlace>
    {
        [OnChangedMethod(nameof(OnSearchQuerryChanged))]
        public string SearchQuerry { get; set; } = String.Empty;

        PaginationParameters pagination { get; set; }

        public IncrementalPlacesSearch()
        {
            pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaPlace>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.LocationProcessor.SearchPlacesAsync(SearchQuerry, pagination,
                    cancellationToken: cancellationToken);
                if (!result.Succeeded) throw result.Info.Exception;
                HasMoreAvailable = result.Value.HasMore;
                return result.Value.Items;
            }
        }

        void OnSearchQuerryChanged()
        {
            pagination = PaginationParameters.MaxPagesToLoad(1);
            HasMoreAvailable = true;
        }
    }
}
