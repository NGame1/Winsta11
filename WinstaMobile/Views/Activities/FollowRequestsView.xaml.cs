using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Helpers.ExtensionMethods;
using WinstaCore.Interfaces.Views.Activities;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore;
using Microsoft.Extensions.DependencyInjection;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Activities
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FollowRequestsView : BasePage, IFollowRequestsView
    {
        public override string PageHeader { get; protected set; }
        public FollowRequestsView()
        {
            this.InitializeComponent();
        }

        private void TextLinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
            => e.HandleClickEvent();

        private void AcceptFriendshipRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&
                element.DataContext is InstaUserShortFriendship friendship)
                ViewModel.ApproveFollowRequestCommand.Execute(friendship);
        }

        private void RejectFriendshipRequest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element &&
                element.DataContext is InstaUserShortFriendship friendship)
            {
                ViewModel.RejectFollowRequestCommand.Execute(friendship);
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is InstaUserShortFriendship friendship)
            {
                var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
                ViewModel.NavigationService.Navigate(UserProfileView, friendship);
            }
        }
    }
}
