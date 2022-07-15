using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Comments
{
    public class IncrementalCommentLikers : IIncrementalSource<InstaUserShort>
    {
        PaginationParameters Pagination { get; set; }

        public string CommentPk { get; set; }

        public IncrementalCommentLikers(string pk)
        {
            CommentPk = pk;
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaUserShort>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.CommentProcessor.GetMediaCommentLikersAsync(CommentPk);

                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;

                HasMoreAvailable = false;

                return result.Value;
            }
        }
    }
}
