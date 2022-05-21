using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Abstractions.Direct.Models;
using WinstaNext.Constants;
using WinstaNext.Services;
using WinstaNext.UI.Directs;
using WinstaNext.ViewModels.Directs;
using WinstaNext.Views.Directs;

namespace WinstaNext.UI.Flyouts.Directs
{
    internal class DirectMessageItemMenuFlyout : MenuFlyout
    {
        public static readonly DependencyProperty DirectItemProperty = DependencyProperty.Register(
          "DirectItem",
          typeof(InstaDirectInboxItemFullModel),
          typeof(DirectMessageItemMenuFlyout),
          new PropertyMetadata(null));

        public InstaDirectInboxItemFullModel DirectItem
        {
            get { return (InstaDirectInboxItemFullModel)GetValue(DirectItemProperty); }
            set { SetValue(DirectItemProperty, value); }
        }

        string threadId = string.Empty;

        public AsyncRelayCommand UnsendMessageCommand { get; set; }
        public AsyncRelayCommand LikeMessageCommand { get; set; }
        public AsyncRelayCommand UnlikeMessageCommand { get; set; }

        public RelayCommand ReplyCommand { get; set; }

        public DirectMessageItemMenuFlyout()
        {
            UnsendMessageCommand = new(UnsendMessageAsync);
            UnlikeMessageCommand = new(UnlikeMessageAsync);
            LikeMessageCommand = new(LikeMessageAsync);
            ReplyCommand = new(Reply);
            this.Opening += DirectMessageItemMenuFlyout_Opening;
        }

        public void Reply()
        {
            if (DirectThreadViewModel.CurrentVM == null) return;
            DirectThreadViewModel.CurrentVM.RepliedMessage = DirectItem;
        }

        async Task LikeMessageAsync()
        {
            using (var Api = App.Container?.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.LikeThreadMessageAsync(threadId, DirectItem.ItemId);
                if (!result.Succeeded)
                    throw result.Info.Exception;
            }
        }

        async Task UnlikeMessageAsync()
        {
            using (var Api = App.Container?.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.UnLikeThreadMessageAsync(threadId, DirectItem.ItemId);
                if (!result.Succeeded)
                    throw result.Info.Exception;
            }
        }

        async Task UnsendMessageAsync()
        {
            using (var Api = App.Container?.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.DeleteSelfMessageAsync(threadId, DirectItem.ItemId);
                if (!result.Succeeded)
                    throw result.Info.Exception;
            }
        }

        private void DirectMessageItemMenuFlyout_Opening(object sender, object e)
        {
            var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];

            Items.Clear();
            var NavigationService = App.Container.GetService<NavigationService>();
            var Me = App.Container.GetService<InstaUserShort>();
            if (NavigationService.Content is DirectsListView view)
            {
                if (view.ListDetails.SelectedItem is InstaDirectInboxThread thread)
                    threadId = thread.ThreadId;
                else { Hide(); return; }
            }
            else { Hide(); return; }
            Items.Add(new MenuFlyoutItem()
            {
                Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Reply, FontFamily = FluentSystemIconsRegular },
                Text = LanguageManager.Instance.Instagram.Reply,
                Command = ReplyCommand
            });
            if (DirectItem.UserId == Me.Pk)
            {
                //My own message
                Items.Add(new MenuFlyoutItem()
                {
                    Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Delete, FontFamily = FluentSystemIconsRegular },
                    Text = LanguageManager.Instance.Instagram.Unsend,
                    Command = UnsendMessageCommand
                });
            }
            if (!DirectItem.Reactions.Liked)
            {
                Items.Add(new MenuFlyoutItem()
                {
                    Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Heart, FontFamily = FluentSystemIconsRegular },
                    Text = LanguageManager.Instance.Instagram.Like,
                    Command = LikeMessageCommand
                });
            }
            else
            {
                Items.Add(new MenuFlyoutItem()
                {
                    Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Heart, FontFamily = FluentSystemIconsRegular },
                    Text = LanguageManager.Instance.Instagram.Unlike,
                    Command = UnlikeMessageCommand
                });
            }
        }
    }
}
