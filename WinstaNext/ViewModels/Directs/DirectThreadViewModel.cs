using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement.Core;
using Windows.UI.Xaml;
using WinstaNext.Abstractions.Direct.Models;
using WinstaNext.Core.Collections.IncrementalSources.Directs;
using WinstaNext.Views.Directs;

namespace WinstaNext.ViewModels.Directs
{
    public class DirectThreadViewModel : BaseViewModel
    {
        InstaDirectInboxThread DirectThread { get; set; }
        IncrementalDirectThread Instance { get; set; }
        public DirectMessagesInvertedCollection ThreadItems { get; private set; }

        public AsyncRelayCommand SendMessageCommand { get; set; }
        public AsyncRelayCommand SendLikeCommand { get; set; }
        public AsyncRelayCommand<DependencyObject> OpenEmojisPanelCommand { get; set; }

        public string MessageText { get; set; }

        public string ThreadId { get; set; }
        public override string PageHeader { get; protected set; }

        public DirectThreadViewModel(InstaDirectInboxThread directThread)
        {
            DirectThread = directThread;
            Instance = new(DirectThread);
            ThreadItems = new(Instance);
            SendLikeCommand = new(SendLikeAsync);
            SendMessageCommand = new(SendMessageAsync);
            OpenEmojisPanelCommand = new(OpenEmojisPanel);
        }

        ~DirectThreadViewModel()
        {
            DirectThread = null;
            Instance = null;
            ThreadItems = null;
            SendMessageCommand = null;
        }

        async Task SendMessageAsync()
        {
            if (SendMessageCommand.IsRunning) return;
            using (var Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.SendDirectTextAsync(null, ThreadId, MessageText);
                if (result.Succeeded)
                {
                    ThreadItems.InsertNewTextMessage(result.Value, MessageText);
                    MessageText = string.Empty;
                }
            }
        }

        async Task SendLikeAsync()
        {
            if (SendLikeCommand.IsRunning) return;
            using (var Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.SendDirectLikeAsync(ThreadId);
                if (result.Succeeded)
                {
                    ThreadItems.InsertNewLikeMessage();
                }
            }
        }

        async Task OpenEmojisPanel(DependencyObject obj)
        {
            await obj.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                CoreInputView.GetForCurrentView().TryShow(CoreInputViewKind.Emoji);
            });
        }
    }
}
