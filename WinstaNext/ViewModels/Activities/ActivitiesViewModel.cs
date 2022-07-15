using Core.Collections.IncrementalSources.Activities;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace WinstaNext.ViewModels.Activities
{
    internal class ActivitiesViewModel : BaseViewModel
    {
        IncrementalUserActivities Instance { get; set; }

        public IncrementalLoadingCollection<IncrementalUserActivities, InstaRecentActivityFeed> Activities { get; }

        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Activities;

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
            if(lastCheck != null)
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
