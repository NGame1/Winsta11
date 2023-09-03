using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Directs
{
    public class IncrementalDirectInbox : IIncrementalSource<InstaDirectInboxThread>
    {
        PaginationParameters Pagination { get; }

        public int SeqId { get; private set; }
        public DateTime SnapshotAt { get; private set; }

        public IncrementalDirectInbox()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool nomoreitems = false;
        bool _isLoading = false;
        public async Task<IEnumerable<InstaDirectInboxThread>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (nomoreitems) return null;
            using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
            while (_isLoading)
            {
                await Task.Delay(200);
            }
            try
            {
                var currentNextMaxId = Pagination.NextMaxId;
                _isLoading = true;
                var result = await Api.MessagingProcessor.GetDirectInboxAsync(Pagination,
                cancellationToken: cancellationToken);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                if (!result.Value.Inbox.HasOlder) nomoreitems = true;
                if (result.Value.SeqId > 0)
                    SeqId = result.Value.SeqId;
                SnapshotAt = result.Value.SnapshotAt;
                if (currentNextMaxId == Pagination.NextMaxId)
                    return null;
                return result.Value.Inbox.Threads;
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}
