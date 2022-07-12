using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using WinstaNext.Abstractions.Direct.Models;
#nullable enable

namespace WinstaNext.Abstractions.Direct.Converters
{
    internal class InstaDirectInboxItemListConverter : WinstaObjectConverter<List<InstaDirectInboxItemFullModel>, InstaDirectInboxThread>
    {
        public InstaDirectInboxThread? SourceObject { get; set; }

        public List<InstaDirectInboxItemFullModel> Convert()
        {
            if (SourceObject == null) throw new ArgumentNullException(nameof(SourceObject));
            var me = App.Container?.GetRequiredService<IInstaApi>().GetLoggedUser().LoggedInUser;
            var lst = new List<InstaDirectInboxItemFullModel>();

            var users = SourceObject.Users.ToDictionary(x => x.Pk);
            for (int i = 0; i < SourceObject.Items.Count; i++)
            {
                var item = SourceObject.Items.ElementAt(i);
                var find = users.Where(x => x.Key == item.UserId);
                InstaUserShort? user = null;
                if (find.Any())
                    user = find.FirstOrDefault().Value;
                else user = me;
                var newItem = new InstaDirectInboxItemFullModel(item, user);
                lst.Add(newItem);
            }
            return lst;
        }


    }
}
