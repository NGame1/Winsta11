using WinstaNext.Abstractions.Stories;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StoryCarouselView : BasePage
    {
        public StoryCarouselView()
        {
            this.InitializeComponent();
        }

        private void StoryItemView_ItemsEnded(object sender, WinstaReelFeed e) => ViewModel.NextStory(e);
    }
}
