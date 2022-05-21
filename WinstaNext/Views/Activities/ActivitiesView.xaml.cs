using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
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
using WinstaNext.Helpers.ExtensionMethods;
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
            => obj.HandleClickEvent();

        private void AcceptFriendshipRequest_Click(object sender, RoutedEventArgs e)
        {
            var dt = (sender as FrameworkElement).DataContext;
            if (dt is InstaRecentActivityFeed activityFeed)
                ViewModel.ApproveFollowRequestCommand.Execute(activityFeed);
        }

        private void RejectFriendshipRequest_Click(object sender, RoutedEventArgs e)
        {
            var dt = (sender as FrameworkElement).DataContext;
            if (dt is InstaRecentActivityFeed activityFeed)
                ViewModel.RejectFollowRequestCommand.Execute(activityFeed);
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is InstaRecentActivityFeed activityFeed)
                if (activityFeed.Type == InstaActivityFeedType.Follow)
                    ViewModel.NavigationService.Navigate(typeof(FollowRequestsView));
        }
    }
}
