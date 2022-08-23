using System;
using Windows.System;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore.Services;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Text.RegularExpressions;
using WinstaCore.Constants;
using InstagramApiSharp.Classes;
using System.Threading;

namespace WinstaCore.Helpers.ExtensionMethods
{
    public static class LinkClickedEventExtensions
    {
        public static async void HandleClickEvent(this LinkClickedEventArgs obj, NavigationService navigation = null)
        {
            NavigationService NavigationService;
            var matches = Regex.Matches(obj.Link.ToLower(), RegexConstants.WebUrlRegex);
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
            else if (Regex.Match(obj.Link, RegexConstants.WebUrlRegex) is Match match && match.Groups.Count > 1)
            {
                await Launcher.LaunchUriAsync(new Uri(match.Value, UriKind.RelativeOrAbsolute));
            }
            else if (Regex.Match(obj.Link, RegexConstants.WebUrlWithoutPrefixRegex) is Match match2 && match2.Groups.Count > 1)
            {
                await Launcher.LaunchUriAsync(new Uri(match2.Value, UriKind.RelativeOrAbsolute));
            }
            //else if (obj.Link.StartsWith("http://") || obj.Link.StartsWith("https://"))
            //{
            //    await Launcher.LaunchUriAsync(new Uri(obj.Link, UriKind.RelativeOrAbsolute));
            //}
        }
    }
}
