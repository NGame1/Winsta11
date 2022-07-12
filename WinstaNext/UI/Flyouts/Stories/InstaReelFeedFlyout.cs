using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

namespace WinstaNext.UI.Flyouts.Stories
{
    internal class InstaReelFeedFlyout : MenuFlyout
    {
        public static readonly DependencyProperty ReelFeedProperty = DependencyProperty.Register(
          nameof(ReelFeed),
          typeof(InstaReelFeed),
          typeof(InstaBroadcastFlyout),
          new PropertyMetadata(null));

        public InstaReelFeed ReelFeed
        {
            get { return (InstaReelFeed)GetValue(ReelFeedProperty); }
            set { SetValue(ReelFeedProperty, value); }
        }

        RelayCommand NavigateToUserProfileCommand { get; set; }

        public InstaReelFeedFlyout()
        {
            Opening += InstaReelFeedFlyout_Opening;
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
        }

        void NavigateToUserProfile()
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), ReelFeed.User);
        }

        private void InstaReelFeedFlyout_Opening(object sender, object e)
        {
            if (ReelFeed == null) return;
            Items.Clear();

            //var font = (FontFamily)App.Current.Resources["FluentIcons"];
            var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];

            if(ReelFeed.User != null)
            {
                Items.Add(new MenuFlyoutItem()
                {
                    Icon = new FontIcon() { Glyph = "\uF5BE", FontFamily = FluentSystemIconsRegular },
                    Text = LanguageManager.Instance.Instagram.ViewProfile,
                    Command = NavigateToUserProfileCommand
                });
            }

            if (!Items.Any())
                this.Hide();
        }
    }
}
