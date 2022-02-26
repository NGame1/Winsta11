using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace WinstaNext.Core.Collections.IncrementalSources.Directs
{
    internal class IncrementalDirectInbox : IIncrementalSource<InstaDirectInboxThread>
    {
        PaginationParameters Pagination { get; }

        public IncrementalDirectInbox()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool nomoreitems = false;
        public async Task<IEnumerable<InstaDirectInboxThread>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (nomoreitems) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.GetDirectInboxAsync(Pagination, 
                    cancellationToken: cancellationToken);
                if (!result.Succeeded) throw result.Info.Exception;
                if (!result.Value.Inbox.HasOlder) nomoreitems = true;
                return result.Value.Inbox.Threads;
            }
        }
    }
}
