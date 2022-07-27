using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;
#nullable enable

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Profiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class HashtagProfileView : BasePage, IHashtagProfileView
    {
        public override string PageHeader { get; protected set; }

        ItemsWrapGrid? WrapGrid { get; set; }
        public HashtagProfileView()
        {
            this.InitializeComponent();
        }

        private void lst_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ListViewScroll = lst.FindDescendantOrSelf<ScrollViewer>();
        }

        private void ItemsWrapGrid_Loaded(object sender, RoutedEventArgs e)
        {
            WrapGrid = (ItemsWrapGrid)sender;
        }

        private void lst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WrapGrid == null) return;
            if(!ApplicationSettingsManager.Instance.GetForceThreeColumns())
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
