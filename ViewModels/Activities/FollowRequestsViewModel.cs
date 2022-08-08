using Core.Collections.IncrementalSources.Activities;
using Core.Collections.IncrementalSources.Search;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;

namespace ViewModels.Activities
{
    public class FollowRequestsViewModel : BaseViewModel
    {
        public IncrementalPendingFriendRequests Instance { get; set; }
        public IncrementalLoadingCollection<IncrementalPendingFriendRequests, InstaUserShortFriendship> FriendshipRequests { get; set; }

        public AsyncRelayCommand<InstaUserShortFriendship> ApproveFollowRequestCommand { get; set; }
        public AsyncRelayCommand<InstaUserShortFriendship> RejectFollowRequestCommand { get; set; }
        public RelayCommand<ItemClickEventArgs> NavigateToUserCommand { get; set; }

        public FollowRequestsViewModel()
        {
            ApproveFollowRequestCommand = new(ApproveFollowRequestAsync);
            RejectFollowRequestCommand = new(RejectFollowRequestAsync);
            NavigateToUserCommand = new(NavigateToUser);
            Instance = new();
            FriendshipRequests = new(Instance);
        }

        void NavigateToUser(ItemClickEventArgs obj)
        {
            var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
            NavigationService.Navigate(UserProfileView, obj.ClickedItem);
        }

        async Task ApproveFollowRequestAsync(InstaUserShortFriendship obj)
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.AcceptFriendshipRequestAsync(obj.Pk);
                if (result.Succeeded)
                {
                    if (obj.FriendshipStatus.Following || obj.FriendshipStatus.OutgoingRequest)
                        FriendshipRequests.Remove(obj);
                }
            }
        }

        async Task RejectFollowRequestAsync(InstaUserShortFriendship obj)
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.IgnoreFriendshipRequestAsync(obj.Pk);
                if (result.Succeeded)
                    FriendshipRequests.Remove(obj);
            }
        }
    }
}
