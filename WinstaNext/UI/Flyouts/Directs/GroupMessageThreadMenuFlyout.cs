using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Resources;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Constants;

namespace WinstaNext.UI.Flyouts.Directs
{
    internal class GroupMessageThreadMenuFlyout : MenuFlyout
    {
        public static readonly DependencyProperty DirectThreadProperty = DependencyProperty.Register(
          nameof(DirectThread),
          typeof(InstaDirectInboxThread),
          typeof(GroupMessageThreadMenuFlyout),
          new PropertyMetadata(null));

        public InstaDirectInboxThread DirectThread
        {
            get { return (InstaDirectInboxThread)GetValue(DirectThreadProperty); }
            set { SetValue(DirectThreadProperty, value); }
        }

        public AsyncRelayCommand DeleteThreadCommand { get; set; }
        public AsyncRelayCommand MuteCommand { get; set; }
        public AsyncRelayCommand UnmuteCommand { get; set; }

        public GroupMessageThreadMenuFlyout()
        {
            DeleteThreadCommand = new(DeleteAsync);
            UnmuteCommand = new(UnmuteAsync);
            MuteCommand = new(MuteAsync);
            this.Opening += PrivateMessageThreadMenuFlyout_Opening;
        }

        private void PrivateMessageThreadMenuFlyout_Opening(object sender, object e)
        {
            var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];

            Items.Clear();
            if (!DirectThread.IsGroup) return;

            if (!DirectThread.Muted)
                Items.Add(new MenuFlyoutItem()
                {
                    Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.SpeakerMute, FontFamily = FluentSystemIconsRegular },
                    Text = LanguageManager.Instance.Instagram.MuteConversation,
                    Command = MuteCommand
                });
            else
                Items.Add(new MenuFlyoutItem()
                {
                    Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Speaker0, FontFamily = FluentSystemIconsRegular },
                    Text = LanguageManager.Instance.Instagram.UnmuteConversation,
                    Command = UnmuteCommand
                });

            Items.Add(new MenuFlyoutItem()
            {
                Icon = new FontIcon() { Glyph = FluentRegularFontCharacters.Delete, FontFamily = FluentSystemIconsRegular },
                Text = LanguageManager.Instance.Instagram.DeleteThread,
                Command = DeleteThreadCommand
            });
        }

        async Task DeleteAsync()
        {
            var msg = new MessageDialog(LanguageManager.Instance.Messages.LeaveGroupContent, LanguageManager.Instance.Messages.LeaveConfirmTitle);
            msg.Commands.Add(new UICommand(LanguageManager.Instance.General.Yes, null, "yes"));
            msg.Commands.Add(new UICommand(LanguageManager.Instance.General.No, null, null));
            var res = await msg.ShowAsync();
            if (res.Id == null) return;

            try
            {
                if (DeleteThreadCommand.IsRunning) return;
                using (var Api = App.Container?.GetService<IInstaApi>())
                {
                    var result = await Api.MessagingProcessor.LeaveGroupThreadAsync(DirectThread.ThreadId);
                    if (!result.Succeeded)
                        throw result.Info.Exception;
                }
            }
            finally
            {
                this.Hide();
            }
        }

        async Task MuteAsync()
        {
            if (MuteCommand.IsRunning) return;
            using (var Api = App.Container?.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.MuteDirectThreadMessagesAsync(DirectThread.ThreadId);
                if (!result.Succeeded)
                    throw result.Info.Exception;
                else DirectThread.Muted = !DirectThread.Muted;
            }
        }

        async Task UnmuteAsync()
        {
            if (UnmuteCommand.IsRunning) return;
            using (var Api = App.Container?.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.UnMuteDirectThreadMessagesAsync(DirectThread.ThreadId);
                if (!result.Succeeded)
                    throw result.Info.Exception;
                else DirectThread.Muted = !DirectThread.Muted;
            }
        }

    }
}
