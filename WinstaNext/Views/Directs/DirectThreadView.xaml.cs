using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Abstractions.Direct.Models;
using WinstaNext.Core.Collections.IncrementalSources.Directs;

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

        IncrementalDirectThread Instance { get; set; }
        public DirectMessagesInvertedCollection ThreadItems { get; private set; }
        public static double MessageBubbleMaxWidth { get; private set; }
        public DirectThreadView()
        {
            this.InitializeComponent();
        }

        void OnDirectItemChanged()
        {
            Instance = new IncrementalDirectThread(DirectThread);
            ThreadItems =
                new DirectMessagesInvertedCollection(Instance);
            lst.ItemsSource = ThreadItems;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MessageBubbleMaxWidth = e.NewSize.Width - 150;
        }

        private async void lst_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {
            var last = ThreadItems[ThreadItems.Count - 1];
            await lst.SmoothScrollIntoViewWithItemAsync(last, itemPlacement: ScrollItemPlacement.Bottom);
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
            base.InsertItem(0, item);
        }

    }
}
