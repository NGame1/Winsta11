﻿using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Direct.Models;
using WinstaCore;
using Abstractions.Direct.Converters;
using InstagramApiSharp.Classes;

namespace Core.Collections.IncrementalSources.Directs
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
            if (pageIndex == 0)
            {
                InboxThread.Items.RemoveAll(x => x.ItemType == InstaDirectThreadItemType.ActionLog);
                return Convert(InboxThread);
            }
            if (!hassOlder) return null;
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.GetDirectInboxThreadAsync(InboxThread.ThreadId, Pagination,
                    cancellationToken: cancellationToken,
                    seqId: seqId);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                hassOlder = result.Value.HasOlder;
                if (pageIndex == 1)
                {
                    await MarkThreadAsSeen();
                    for (int i = 0; i < InboxThread.Items.Count; i++)
                    {
                        var found = result.Value.Items.FirstOrDefault(x => x.ItemId == InboxThread.Items.ElementAt(i).ItemId);
                        if (found != null)
                            result.Value.Items.Remove(found);
                    }
                }
                result.Value.Items.RemoveAll(x => x.ItemType == InstaDirectThreadItemType.ActionLog);
                result.Value.Items.Reverse();
                return Convert(result.Value);
            }
        }

        async Task MarkThreadAsSeen()
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                await Api.MessagingProcessor.MarkDirectThreadAsSeenAsync(InboxThread.ThreadId, InboxThread.Items.ElementAt(0).ItemId);
            }
        }

        public IEnumerable<InstaDirectInboxItemFullModel> Convert(
           InstaDirectInboxThread inboxThread)
        {
            return new InstaDirectInboxItemListConverter { SourceObject = inboxThread }.Convert();
        }
    }
}
