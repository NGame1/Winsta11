using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml.Data;
using WinstaCore.Services;
using WinstaNext.Services;
using WinstaNext.ViewModels.Users;
using WinstaNext.Views.Profiles;

namespace WinstaNext.Converters.Profiles
{
    internal class UserInfoToButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not InstaStoryFriendshipStatus FriendshipStatus || FriendshipStatus == null) return string.Empty;
            if (App.Container.GetService<NavigationService>().Content is UserProfileView userProfile)
            {
                if (userProfile.DataContext is UserProfileViewModel viewModel)
                    if (viewModel.User.Pk == App.Container.GetService<InstaUserShort>().Pk)
                        return LanguageManager.Instance.Instagram.EditProfile;
            }
            //if (User.Pk == App.Container.GetService<InstaUserShort>().Pk)
            //    return LanguageManager.Instance.Instagram.EditProfile;
            if (FriendshipStatus.OutgoingRequest)
                return LanguageManager.Instance.Instagram.Requested;
            else if (!FriendshipStatus.Following && !FriendshipStatus.FollowedBy)
                return LanguageManager.Instance.Instagram.Follow;
            else if (!FriendshipStatus.Following && FriendshipStatus.FollowedBy)
                return LanguageManager.Instance.Instagram.FollowBack;
            else if (FriendshipStatus.Following)
                return LanguageManager.Instance.Instagram.Unfollow;
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
