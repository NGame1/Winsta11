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
using WinstaNext.Abstractions;
using WinstaNext.Abstractions.Direct.Converters;
using WinstaNext.Abstractions.Direct.Models;

namespace WinstaNext.Core.Collections.IncrementalSources.Directs
{
    public class IncrementalDirectThread : IIncrementalSource<InstaDirectInboxItemFullModel>
    {
        PaginationParameters Pagination { get; set; }
        int seqId { get; }
        InstaDirectInboxThread InboxThread { get; }

        public IncrementalDirectThread(InstaDirectInboxThread inboxThread)
        {
            InboxThread = inboxThread;
            //seqId = seqid;
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool hassOlder = true;
        public async Task<IEnumerable<InstaDirectInboxItemFullModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!hassOlder) return null;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.GetDirectInboxThreadAsync(InboxThread.ThreadId, Pagination,
                    cancellationToken: cancellationToken,
                    seqId: seqId);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                hassOlder = result.Value.HasOlder;
                result.Value.Items.Reverse();
                return Convert(result.Value);
            }
        }

        public IEnumerable<InstaDirectInboxItemFullModel> Convert(
           InstaDirectInboxThread inboxThread)
        {
            return new InstaDirectInboxItemListConverter { SourceObject = inboxThread }.Convert();
        }
    }
}
