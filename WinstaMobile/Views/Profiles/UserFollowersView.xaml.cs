using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;
using Microsoft.Extensions.DependencyInjection;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Profiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserFollowersView : BasePage, IUserFollowersView
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Followers;

        public UserFollowersView()
        {
            this.InitializeComponent();
        }

        private async void InstaUserPresenterUC_RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btnRemove) return;
            if (btnRemove.DataContext is not InstaUserShort userShort) return;
            using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
            var result = await Api.UserProcessor.RemoveFollowerAsync(userShort.Pk);
            if (!result.Succeeded) return;
            ViewModel.UserFollowers.Remove(userShort);
        }
    }
}
