using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinstaNext.Abstractions.Direct.Models;

namespace WinstaNext.Abstractions.Direct.Converters
{
    internal class InstaDirectInboxItemListConverter : WinstaObjectConverter<List<InstaDirectInboxItemFullModel>, InstaDirectInboxThread>
    {
        public InstaDirectInboxThread SourceObject { get; set; }

        public List<InstaDirectInboxItemFullModel> Convert()
        {
            var me = App.Container.GetService<IInstaApi>().GetLoggedUser().LoggedInUser;
            var lst = new List<InstaDirectInboxItemFullModel>();

            var users = SourceObject.Users.ToDictionary(x => x.Pk);
            for (int i = 0; i < SourceObject.Items.Count; i++)
            {
                var item = SourceObject.Items.ElementAt(i);
                var newItem = new InstaDirectInboxItemFullModel(item);
                var find = users.Where(x => x.Key == item.UserId);
                if (find.Any())
                    newItem.User = find.FirstOrDefault().Value;
                else newItem.User = me;
                lst.Add(newItem);
            }
            return lst;
        }


    }
}
