using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinstaNext.Core.Collections.IncrementalSources.Comments
{
    public class IncrementalMediaComments : IIncrementalSource<InstaComment>
    {
        PaginationParameters Pagination { get; set; }

        public string MediaId { get; }
        public string TargetCommentId { get; }

        public IncrementalMediaComments(string mediaId, string targetCommentId = "")
        {
            MediaId = mediaId;
            TargetCommentId = targetCommentId;
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<InstaComment>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.CommentProcessor.GetMediaCommentsAsync(MediaId, Pagination,
                                   targetCommentId: TargetCommentId);

                if (!result.Succeeded) throw result.Info.Exception;

                HasMoreAvailable = result.Value.MoreHeadLoadAvailable;

                return result.Value.Comments;
            }
        }
    }
}
