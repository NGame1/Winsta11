using Abstractions.Stories;
using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class HashtagStoryItemView : Page
    {

        public static readonly DependencyProperty StoryRootProperty = DependencyProperty.Register(
          nameof(StoryRoot),
          typeof(WinstaStoryItem),
          typeof(HashtagStoryItemView),
          new PropertyMetadata(null));

        public WinstaStoryItem StoryRoot
        {
            get { return (WinstaStoryItem)GetValue(StoryRootProperty); }
            set { SetValue(StoryRootProperty, value); }
        }

        [OnChangedMethod(nameof(OnHashtagStoryItemChanged))]
        public InstaHashtagStory HashtagStoryItem
        {
            get => StoryRoot.HashtagStory;
        }

        public HashtagStoryItemView()
        {
            this.InitializeComponent();
        }

        void OnHashtagStoryItemChanged()
        {

        }
    }
}
