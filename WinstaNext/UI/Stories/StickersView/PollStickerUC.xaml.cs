using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.UI.Stories.StickersView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class PollStickerUC : UserControl
    {
        public static readonly DependencyProperty PollProperty = DependencyProperty.Register(
                nameof(Poll),
                typeof(InstaStoryPollItem),
                typeof(PollStickerUC),
                new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnPollChanged))]
        public InstaStoryPollItem Poll
        {
            get { return (InstaStoryPollItem)GetValue(PollProperty); }
            set { SetValue(PollProperty, value); }
        }

        public PollStickerUC()
        {
            this.InitializeComponent();
        }

        void OnPollChanged()
        {
            if (string.IsNullOrEmpty(Poll.PollSticker.Question))
                titleRow.Height = new(0);
        }
    }
}
