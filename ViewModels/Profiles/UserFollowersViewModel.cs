using Core.Collections.IncrementalSources.Users;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore;
using PropertyChanged;

namespace ViewModels.Profiles
{
    [AddINotifyPropertyChangedInterface]
    public class UserFollowersViewModel : BaseViewModelWithStopwatch
    {
        IncrementalUserFollowers UserFollowersInstance { get; set; }

        [OnChangedMethod(nameof(OnSearchQuerryChanged))]
        public string SearchQuerry { get; set; }

        public IncrementalLoadingCollection<IncrementalUserFollowers, InstaUserShort> UserFollowers { get; set; }

        public RelayCommand<ItemClickEventArgs> NavigateToUserCommand { get; set; }

        public UserFollowersViewModel()
        {
            NavigateToUserCommand = new(NavigateToUser);
        }

        void NavigateToUser(ItemClickEventArgs obj)
        {
            var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
            NavigationService.Navigate(UserProfileView, obj.ClickedItem);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not long UserPk)
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                throw new ArgumentOutOfRangeException(nameof(e.Parameter));
            }
            if (UserFollowersInstance != null && UserFollowersInstance.UserId == UserPk) return;

            UserFollowersInstance = new(UserPk);
            UserFollowers = new(UserFollowersInstance);

            base.OnNavigatedTo(e);
        }

        async void OnSearchQuerryChanged()
        {
            if (!await EnsureTimeElapsed()) return;
            UserFollowersInstance.SearchQuerry = SearchQuerry;
            await UserFollowers.RefreshAsync();
        }

    }
}
