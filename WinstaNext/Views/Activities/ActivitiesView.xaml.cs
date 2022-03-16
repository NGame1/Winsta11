using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ActivitiesView : BasePage
    {
        public ActivitiesView()
        {
            this.InitializeComponent();
        }

        void TextLinkClicked(object sender, LinkClickedEventArgs obj)
        {
            var NavigationService = App.Container.GetService<NavigationService>();
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
        }

    }
}
