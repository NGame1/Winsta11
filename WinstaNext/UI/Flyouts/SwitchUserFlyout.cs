using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Services;

namespace WinstaNext.UI.Flyouts
{
    public sealed partial class SwitchUserFlyout : MenuFlyout
    {
        public SwitchUserFlyout()
        {
            this.Opening += MenuFlyout_Opening;
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            Items.Clear();

            var font = (FontFamily)App.Current.Resources["FluentIcons"];
            var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];

            var addaccountitem = new MenuFlyoutItem()
            {
                Icon = new FontIcon() { Glyph = "\uF5C3", FontFamily = FluentSystemIconsRegular },
                Text = LanguageManager.Instance.Instagram.AddAccount,
            };
            addaccountitem.Click += Addaccountitem_Click;
            Items.Add(addaccountitem);

            Items.Add(new MenuFlyoutSeparator());

            var Users = ApplicationSettingsManager.Instance.GetUsersList();
            for (int i = 0; i < Users.Count; i++)
            {
                var user = Users.ElementAt(i);
                var useritem = new MenuFlyoutItem()
                {
                    Icon = new FontIcon() { Glyph = "\uF5BE", FontFamily = FluentSystemIconsRegular },
                    Text = user.Value,
                    Tag = user.Key,
                };
                useritem.Click += Useritem_Click;
                Items.Add(useritem);
            }
        }

        private void Useritem_Click(object sender, RoutedEventArgs e)
        {
            var mfi = sender as MenuFlyoutItem;
            var userpk = mfi.Tag.ToString();
            AccountManagementService.SwitchToUser(userpk);
        }

        private void Addaccountitem_Click(object sender, RoutedEventArgs e)
        {
            AccountManagementService.LoginToAnotherUser();
        }
    }
}
