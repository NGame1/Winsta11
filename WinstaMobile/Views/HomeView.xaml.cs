using Windows.UI.Xaml.Controls;
using WinstaCore.Attributes;
using WinstaCore.Interfaces.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : Page, IHomeView
    {
        public HomeView()
        {
            this.InitializeComponent();
        }

        public RangePlayerAttribute Medias { get; private set; }
    }
}
