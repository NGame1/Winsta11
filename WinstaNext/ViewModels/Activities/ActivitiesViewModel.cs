using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinstaNext.Core.Collections.IncrementalSources.Activities;

namespace WinstaNext.ViewModels.Activities
{
    internal class ActivitiesViewModel : BaseViewModel
    {
        public IncrementalLoadingCollection<IncrementalUserActivities, InstaRecentActivityFeed> Activities { get; }

        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Activities;

        public AsyncRelayCommand<InstaRecentActivityFeed> ApproveFollowRequestCommand { get; set; }
        public AsyncRelayCommand<InstaRecentActivityFeed> RejectFollowRequestCommand { get; set; }

        public ActivitiesViewModel()
        {
            Activities = new();
            ApproveFollowRequestCommand = new(ApproveFollowRequestAsync);
            RejectFollowRequestCommand = new(RejectFollowRequestAsync);
        }

        async Task ApproveFollowRequestAsync(InstaRecentActivityFeed obj)
        {
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.AcceptFriendshipRequestAsync(obj.ProfileId);
                if (result.Succeeded)
                {
                    if (obj.InlineFollow.IsFollowing || obj.InlineFollow.IsOutgoingRequest)
                        Activities.Remove(obj);
                }
            }
        }

        async Task RejectFollowRequestAsync(InstaRecentActivityFeed obj)
        {
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.IgnoreFriendshipRequestAsync(obj.ProfileId);
                if (result.Succeeded)
                    Activities.Remove(obj);
            }
        }
    }
}
