// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;

namespace WinstaNext.Views.Profiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserFollowingsView : BasePage, IUserFollowingsView
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Followings;

        public UserFollowingsView()
        {
            this.InitializeComponent();
        }

        private async void InstaUserPresenterUC_RemoveButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btnRemove) return;
            if (btnRemove.DataContext is not InstaUserShort userShort) return;
            using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
            var result = await Api.UserProcessor.UnFollowUserAsync(userShort.Pk);
            if (!result.Succeeded) return;
            ViewModel.UserFollowings.Remove(userShort);
        }
    }
}
