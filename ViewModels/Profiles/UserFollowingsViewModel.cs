using Core.Collections.IncrementalSources.Users;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Resources;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;

namespace ViewModels.Profiles
{
    public class UserFollowingsViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Followings;

        IncrementalUserFollowings UserFollowingsInstance { get; set; }

        public IncrementalLoadingCollection<IncrementalUserFollowings, InstaUserShort> UserFollowings { get; set; }
        public RelayCommand<ItemClickEventArgs> NavigateToUserCommand { get; set; }

        public UserFollowingsViewModel()
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
            if (UserFollowingsInstance != null && UserFollowingsInstance.UserId == UserPk) return;

            UserFollowingsInstance = new(UserPk);
            UserFollowings = new(UserFollowingsInstance);

            base.OnNavigatedTo(e);
        }
    }
}
