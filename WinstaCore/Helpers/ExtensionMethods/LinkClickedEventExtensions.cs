using System;
using Windows.System;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore.Services;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace WinstaCore.Helpers.ExtensionMethods
{
    public static class LinkClickedEventExtensions
    {
        public static async void HandleClickEvent(this LinkClickedEventArgs obj, NavigationService navigation = null)
        {
            NavigationService NavigationService;
            if (navigation == null)
                NavigationService = AppCore.Container.GetService<NavigationService>();
            else NavigationService = navigation;
            if (obj.Link.StartsWith("@"))
            {
                var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
                NavigationService.Navigate(UserProfileView,
                                  obj.Link.Replace("@", string.Empty));
            }
            else if (obj.Link.StartsWith("#"))
            {
                var HashtagProfileView = AppCore.Container.GetService<IHashtagProfileView>();
                NavigationService.Navigate(HashtagProfileView,
                                  obj.Link.Replace("#", string.Empty));
            }
            else if (obj.Link.StartsWith("http://") || obj.Link.StartsWith("https://"))
            {
                await Launcher.LaunchUriAsync(new Uri(obj.Link, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
