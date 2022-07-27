// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using Resources;
using WinstaCore.Interfaces.Views.Profiles;

namespace WinstaNext.Views.Profiles
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
    }
}
