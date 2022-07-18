using Core.Collections.IncrementalSources.Users;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Resources;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Views.Profiles;
using ViewModels;

namespace WinstaNext.ViewModels.Users
{
    internal class UserFollowersViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Followers;

        IncrementalUserFollowers UserFollowersInstance { get; set; }

        public IncrementalLoadingCollection<IncrementalUserFollowers, InstaUserShort> UserFollowers { get; set; }

        public RelayCommand<ItemClickEventArgs> NavigateToUserCommand { get; set; }

        public UserFollowersViewModel()
        {
            NavigateToUserCommand = new(NavigateToUser);
        }

        void NavigateToUser(ItemClickEventArgs obj)
        {
            NavigationService.Navigate(typeof(UserProfileView), obj.ClickedItem);
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
    }
}
