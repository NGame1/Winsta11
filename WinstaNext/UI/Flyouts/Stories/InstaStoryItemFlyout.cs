using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Helpers.DownloadUploadHelper;
using WinstaCore.Services;
using WinstaNext.Views.Profiles;
using Resources;
using WinstaCore.Constants;

namespace WinstaNext.UI.Flyouts.Stories
{
    internal class InstaStoryItemFlyout : MenuFlyout
    {
        public static readonly DependencyProperty StoryItemProperty = DependencyProperty.Register(
          nameof(StoryItem),
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
        AsyncRelayCommand DeleteStoryCommand { get; set; }

        public InstaStoryItemFlyout()
        {
            Opening += InstaStoryItemFlyout_Opening;
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            DeleteStoryCommand = new(DeleteAsync);
            DownloadStoryCommand = new(Download);
        }

        async Task DeleteAsync()
        {
            var msg = new MessageDialog(LanguageManager.Instance.Messages.DeleteStoryContent, LanguageManager.Instance.Messages.DeleteConfirmTitle);
            msg.Commands.Add(new UICommand(LanguageManager.Instance.General.Yes, null, "yes"));
            msg.Commands.Add(new UICommand(LanguageManager.Instance.General.No, null, null));
            var result = await msg.ShowAsync();
            if (result.Id == null) return;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var res = await Api.StoryProcessor.DeleteStoryAsync(StoryItem.Id,
                          StoryItem.MediaType == InstaMediaType.Image ? InstaSharingType.Photo : InstaSharingType.Video);

                if (!res.Succeeded)
                    throw res.Info.Exception;
                else this.Hide();
            }
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
            if (StoryItem.User != null)
            {
                var me = App.Container.GetService<InstaUserShort>();
                if (StoryItem.User.Pk == me.Pk)
                {
                    //Add my story related items
                    Items.Add(new MenuFlyoutItem()
                    {
                        Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Delete, FontFamily = FluentSystemIconsRegular },
                        Text = LanguageManager.Instance.Instagram.DeleteStory,
                        Command = DeleteStoryCommand
                    });

                }
                else
                {
                    Items.Add(new MenuFlyoutItem()
                    {
                        Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Person, FontFamily = FluentSystemIconsRegular },
                        Text = LanguageManager.Instance.Instagram.ViewProfile,
                        Command = NavigateToUserProfileCommand
                    });
                }
            }

            Items.Add(new MenuFlyoutItem()
            {
                Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Download, FontFamily = FluentSystemIconsRegular },
                Text = LanguageManager.Instance.General.Download,
                Command = DownloadStoryCommand
            });

            if (!Items.Any()) this.Hide();
        }
    }
}
