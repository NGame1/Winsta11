using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Abstractions.Navigation;
using WinstaCore.Constants;
using WinstaCore.Services;
using WinstaNext.Views.Media;
using WinstaNext.Views.Stories;
using Resources;

namespace WinstaNext.UI.Flyouts.Profiles
{
    internal class UserProfileMenuFlyout : MenuFlyout
    {
        public static readonly DependencyProperty StoriesAndLivesProperty = DependencyProperty.Register(
          nameof(StoriesAndLives),
          typeof(InstaStoryAndLives),
          typeof(UserProfileMenuFlyout),
          new PropertyMetadata(null));

        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
          nameof(User),
          typeof(InstaUserInfo),
          typeof(UserProfileMenuFlyout),
          new PropertyMetadata(null));

        public InstaStoryAndLives StoriesAndLives
        {
            get { return (InstaStoryAndLives)GetValue(StoriesAndLivesProperty); }
            set { SetValue(StoriesAndLivesProperty, value); }
        }

        public InstaUserInfo User
        {
            get { return (InstaUserInfo)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        RelayCommand ViewProfilePictureCommand { get; set; }
        RelayCommand ViewStoriesCommand { get; set; }

        public UserProfileMenuFlyout()
        {
            ViewProfilePictureCommand = new(ViewProfilePicture);
            ViewStoriesCommand = new(ViewStories);
            Opening += UserProfileMenuFlyout_Opening;
        }

        void ViewProfilePicture()
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService?.Navigate(typeof(ImageViewerPage), User?.HdProfilePicUrlInfo.Uri);
        }

        void ViewStories()
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService?.Navigate(typeof(StoryCarouselView),
                new StoryCarouselViewParameter(new(StoriesAndLives.Reel)));
        }

        private void UserProfileMenuFlyout_Opening(object sender, object e)
        {
            Items.Clear();
            var Me = App.Container.GetService<InstaUserShort>();
            var font = (FontFamily)App.Current.Resources["FluentIcons"];
            var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];
            if (User != null)
            {
                Items.Add(new MenuFlyoutItem
                {
                    Icon = new FontIcon()
                    {
                        Glyph = FluentRegularFontCharacters.Image,
                        FontFamily = FluentSystemIconsRegular,
                        FontSize = 24
                    },
                    Text = LanguageManager.Instance.Instagram.ViewProfilePicture,
                    Command = ViewProfilePictureCommand
                });
            }

            if (StoriesAndLives != null)
            {
                if (StoriesAndLives.Reel != null)
                    Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = FluentRegularFontCharacters.Movie,
                            FontFamily = FluentSystemIconsRegular,
                            FontSize = 24
                        },
                        Text = LanguageManager.Instance.Instagram.Stories,
                        Command = ViewStoriesCommand
                    });

            }
            if (!Items.Any()) { Hide(); return; }
        }
    }
}
