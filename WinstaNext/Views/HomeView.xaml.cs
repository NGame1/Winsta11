using Windows.UI.Xaml.Controls;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Core.Navigation;
using WinstaNext.Views.Stories;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : BasePage
    {
        public HomeView()
        {
            this.InitializeComponent();
        }

        private void StoriesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var stories = ViewModel.Stories;
            ViewModel.NavigationService.Navigate(typeof(StoryCarouselView), new StoryCarouselViewParameter((WinstaStoryItem)e.ClickedItem, ref stories));
        }
    }
}
