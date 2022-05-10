using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Helpers.DownloadUploadHelper;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

namespace WinstaNext.UI.Flyouts.Stories
{
    internal class InstaStoryItemFlyout : MenuFlyout
    {
        public static readonly DependencyProperty StoryItemProperty = DependencyProperty.Register(
          "StoryItem",
          typeof(InstaStoryItem),
          typeof(InstaStoryItemFlyout),
          new PropertyMetadata(null));

        public InstaStoryItem StoryItem
        {
            get { return (InstaStoryItem)GetValue(StoryItemProperty); }
            set { SetValue(StoryItemProperty, value); }
        }

        RelayCommand NavigateToUserProfileCommand { get; set; }
        RelayCommand DownloadStoryCommand { get; set; }

        public InstaStoryItemFlyout()
        {
            Opening += InstaStoryItemFlyout_Opening;
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            DownloadStoryCommand = new(Download);
        }

        void Download()
        {
            DownloadHelper.Download(StoryItem);
        }

        void NavigateToUserProfile()
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), StoryItem.User);
        }

        private void InstaStoryItemFlyout_Opening(object sender, object e)
        {
            if (StoryItem == null) return;
            Items.Clear();

            //var font = (FontFamily)App.Current.Resources["FluentIcons"];
            var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];
            if(StoryItem.User != null)
            {
                var me = App.Container.GetService<InstaUserShort>();
                if(StoryItem.User.Pk == me.Pk)
                {
                    //Add my story related items

                }
                else
                {
                    Items.Add(new MenuFlyoutItem()
                    {
                        Icon = new FontIcon() { Glyph = "\uF5BE", FontFamily = FluentSystemIconsRegular },
                        Text = LanguageManager.Instance.Instagram.ViewProfile,
                        Command = NavigateToUserProfileCommand
                    });
                }
            }

            Items.Add(new MenuFlyoutItem()
            {
                Icon = new FontIcon() { Glyph = "\uF151", FontFamily = FluentSystemIconsRegular },
                Text = LanguageManager.Instance.General.Download,
                Command = DownloadStoryCommand
            });

            if (!Items.Any()) this.Hide();
        }
    }
}
