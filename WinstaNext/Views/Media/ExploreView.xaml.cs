using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using WinstaCore.Interfaces.Views.Medias;
#nullable enable

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Media
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExploreView : Page, IExploreView
    {
        ItemsWrapGrid? WrapGrid { get; set; }

        public ExploreView()
        {
            this.InitializeComponent();
        }

        private void ItemsWrapGrid_Loaded(object sender, RoutedEventArgs e)
        {
            WrapGrid = (ItemsWrapGrid)sender;
        }

        private void lst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WrapGrid == null) return;
            if (!ApplicationSettingsManager.Instance.GetForceThreeColumns())
            {
                WrapGrid.ItemHeight = WrapGrid.ItemWidth = 185;
                WrapGrid.SizeChanged -= lst_SizeChanged;
                lst.SizeChanged -= lst_SizeChanged;
                return;
            }
            var width = e.NewSize.Width / 3f;
            WrapGrid.ItemHeight = WrapGrid.ItemWidth = width;
        }

    }
}
