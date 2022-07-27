using Core.Collections.IncrementalSources.Activities;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Resources;
using WinstaCore;

namespace ViewModels.Activities
{
    public class ActivitiesViewModel : BaseViewModel
    {
        IncrementalUserActivities Instance { get; set; }

        public IncrementalLoadingCollection<IncrementalUserActivities, InstaRecentActivityFeed> Activities { get; }

        public AsyncRelayCommand<InstaRecentActivityFeed> ApproveFollowRequestCommand { get; set; }
        public AsyncRelayCommand<InstaRecentActivityFeed> RejectFollowRequestCommand { get; set; }
        static DateTime? lastCheck = null;
        public ActivitiesViewModel()
        {
            Instance = new();
            Activities = new(Instance);
            ApproveFollowRequestCommand = new(ApproveFollowRequestAsync);
            RejectFollowRequestCommand = new(RejectFollowRequestAsync);
        }

        public override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (lastCheck != null)
            {
                if (DateTime.Now.Subtract(lastCheck.Value).TotalMinutes < 2) return;
                Instance.RequestRefresh();
                await Activities.RefreshAsync();
            }
            lastCheck = DateTime.Now;
            base.OnNavigatedTo(e);
        }

        async Task ApproveFollowRequestAsync(InstaRecentActivityFeed obj)
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
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
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.IgnoreFriendshipRequestAsync(obj.ProfileId);
                if (result.Succeeded)
                    Activities.Remove(obj);
            }
        }
    }
}
