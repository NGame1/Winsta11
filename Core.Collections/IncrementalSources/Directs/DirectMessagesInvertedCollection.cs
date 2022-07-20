using Abstractions.Direct.Models;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;
using Microsoft.Toolkit.Uwp;
using System;
using WinstaCore;

namespace Core.Collections.IncrementalSources.Directs
{
    public class DirectMessagesInvertedCollection : IncrementalLoadingCollection<IncrementalDirectThread, InstaDirectInboxItemFullModel>
    {
        public DirectMessagesInvertedCollection(IncrementalDirectThread para) : base(para)
        {
        }

        protected override void InsertItem(int index, InstaDirectInboxItemFullModel item)
        {
            if (index != -2)
                base.InsertItem(0, item);
            else base.InsertItem(this.Count, item);
        }

        public void InsertNewTextMessage(InstaDirectRespondPayload payload, string textMessage)
        {
            var me = AppCore.Container.GetService<InstaUserShort>();
            var msg = new InstaDirectInboxItem()
            {
                ClientContext = payload.ClientContext,
                ItemType = InstaDirectThreadItemType.Text,
                ItemId = payload.ItemId,
                Text = textMessage,
                TimeStamp = DateTimeHelper.UnixTimestampMilisecondsToDateTime(payload.Timestamp),
                TimeStampUnix = payload.Timestamp,
                UserId = me.Pk
            };
            InsertItem(-2, new(msg, me));
        }

        public void InsertNewLikeMessage()
        {
            var me = AppCore.Container.GetService<InstaUserShort>();
            var msg = new InstaDirectInboxItem()
            {
                ItemType = InstaDirectThreadItemType.Like,
                TimeStamp = DateTime.UtcNow,
                TimeStampUnix = DateTimeHelper.ToUnixTime(DateTime.UtcNow).ToString(),
                UserId = me.Pk
            };
            InsertItem(-2, new(msg, me));
        }
    }
}
