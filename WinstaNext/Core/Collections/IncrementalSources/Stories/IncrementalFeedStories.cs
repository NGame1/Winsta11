using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
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
using WinstaNext.Abstractions.Stories;

namespace WinstaNext.Core.Collections.IncrementalSources.Stories
{
    public class IncrementalFeedStories : IIncrementalSource<WinstaStoryItem>
    {
        PaginationParameters Pagination { get; set; }


        [OnChangedMethod(nameof(OnRefreshRequestedChanged))]
        public bool RefreshRequested { get; set; }

        public IncrementalFeedStories()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<WinstaStoryItem>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            IResult<InstaStoryFeed> result;
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    result = await Api.StoryProcessor.GetStoryFeedWithPostMethodAsync(
                             cancellationToken: cancellationToken,
                             paginationParameters: Pagination,
                             forceRefresh: RefreshRequested);
                }

                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;

                HasMoreAvailable = Pagination.NextMaxId != null;
                List<WinstaStoryItem> Stories = new();

                var ownStories = result.Value.Items.Where(x => x.User.Pk == App.Container.GetService<InstaUserShort>().Pk);
                if (ownStories.Count() > 1)
                {
                    var own = ownStories.FirstOrDefault();
                    result.Value.Items.RemoveAll(x => x.User.Pk == App.Container.GetService<InstaUserShort>().Pk);
                    result.Value.Items.Insert(0, own);
                }

                //for (int i = 0; i < result.Value.Broadcasts.Count; i++)
                //{
                //    Stories.Add(new(result.Value.Broadcasts.ElementAt(i)));
                //}

                for (int i = 0; i < result.Value.Items.Count; i++)
                {
                    Stories.Add(new(result.Value.Items.ElementAt(i)));
                }

                for (int i = 0; i < result.Value.HashtagStories.Count; i++)
                {
                    Stories.Add(new(result.Value.HashtagStories.ElementAt(i)));
                }

                return Stories;
            }
            finally { }
        }

        void OnRefreshRequestedChanged()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            RefreshRequested = true;
        }
    }
}
