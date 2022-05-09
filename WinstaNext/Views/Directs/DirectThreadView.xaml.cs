using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinstaNext.Abstractions.Direct.Models;
using WinstaNext.Core.Collections.IncrementalSources.Directs;
using WinstaNext.ViewModels.Directs;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.Views.Directs
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class DirectThreadView : Page
    {
        public static readonly DependencyProperty DirectThreadProperty = DependencyProperty.Register(
          "DirectThread",
          typeof(InstaDirectInboxThread),
          typeof(DirectThreadView),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnDirectItemChanged))]
        public InstaDirectInboxThread DirectThread
        {
            get { return (InstaDirectInboxThread)GetValue(DirectThreadProperty); }
            set
            {
                SetValue(DirectThreadProperty, value);
            }
        }

        public static double MessageBubbleMaxWidth { get; private set; }

        public DirectThreadViewModel ViewModel { get; set; }

        public DirectThreadView()
        {
            this.InitializeComponent();
        }

        ~DirectThreadView()
        {
            ViewModel = null;
        }

        void OnDirectItemChanged()
        {
            if (DirectThread == null) return;
            ViewModel = new(DirectThread);
            ViewModel.ThreadId = DirectThread.ThreadId;
            lst.ItemsSource = ViewModel.ThreadItems;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MessageBubbleMaxWidth = e.NewSize.Width - 150;
        }

        private void SendMessageKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.SendMessageCommand.Execute(null);
            args.Handled = true;
        }
    }

    public class DirectMessagesInvertedCollection : IncrementalLoadingCollection<IncrementalDirectThread, InstaDirectInboxItemFullModel>
    {
        InstaDirectInboxItemFullModel LatestMessage { get; set; }

        public DirectMessagesInvertedCollection(IncrementalDirectThread para) : base(para)
        {
        }

        protected override void InsertItem(int index, InstaDirectInboxItemFullModel item)
        {
            if (index != -2)
                base.InsertItem(0, item);
            else base.InsertItem(this.Count, item);
        }

        public void InsertNewTextMessage(InstaDirectRespondPayload payload, string textMessage)
        {
            var me = App.Container.GetService<InstaUserShort>();
            var msg = new InstaDirectInboxItem()
            {
                ClientContext = payload.ClientContext,
                ItemType = InstaDirectThreadItemType.Text,
                ItemId = payload.ItemId,
                Text = textMessage,
                TimeStamp = DateTimeHelper.UnixTimestampMilisecondsToDateTime(payload.Timestamp),
                TimeStampUnix = payload.Timestamp,
                UserId = me.Pk
            };
            InsertItem(-2, new(msg, me));
        }

        public void InsertNewLikeMessage()
        {
            var me = App.Container.GetService<InstaUserShort>();
            var msg = new InstaDirectInboxItem()
            {
                ItemType = InstaDirectThreadItemType.Like,
                TimeStamp = DateTime.UtcNow,
                TimeStampUnix = DateTimeHelper.ToUnixTime(DateTime.UtcNow).ToString(),
                UserId = me.Pk
            };
            InsertItem(-2, new(msg, me));
        }
    }
}
