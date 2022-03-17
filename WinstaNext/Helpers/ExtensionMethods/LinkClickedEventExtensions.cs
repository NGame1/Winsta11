using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

namespace WinstaNext.Helpers.ExtensionMethods
{
    public static class LinkClickedEventExtensions
    {
        public static async void HandleClickEvent(this LinkClickedEventArgs obj, NavigationService navigation = null)
        {
            NavigationService NavigationService;
            if (navigation == null)
                NavigationService = App.Container.GetService<NavigationService>();
            else NavigationService = navigation;
            if (obj.Link.StartsWith("@"))
            {
                NavigationService.Navigate(typeof(UserProfileView),
                                  obj.Link.Replace("@", string.Empty));
            }
            else if (obj.Link.StartsWith("#"))
            {
                NavigationService.Navigate(typeof(HashtagProfileView),
                                  obj.Link.Replace("#", string.Empty));
            }
            else if(obj.Link.StartsWith("http://")|| obj.Link.StartsWith("https://"))
            {
                await Launcher.LaunchUriAsync(new Uri(obj.Link, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
