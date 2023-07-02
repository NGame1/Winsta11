using Resources;
using System.Threading.Tasks;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Helpers.ExtensionMethods;
using WinstaCore.Interfaces.Views.Medias;
#nullable enable

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Media;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ExploreView : BasePage, IExploreView
{
    public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Explore;

    ItemsWrapGrid? WrapGrid { get; set; }

    public ExploreView()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.NavigationMode != NavigationMode.New) return;
        if (ApplicationSettingsManager.Instance.GetAppUiMode() == WinstaCore.Enums.ApplicationUserInterfaceModel.List) return;
        try
        {
            
            await wlst.Medias?.LoadMoreItemsAsync(1);
            base.OnNavigatedTo(e);
        }
        catch (Exception) { }
    }

    private void ItemsWrapGrid_Loaded(object sender, RoutedEventArgs e)
    {
        WrapGrid = (ItemsWrapGrid)sender;
    }

    private void lst_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        //if (WrapGrid == null) return;
        //if (!ApplicationSettingsManager.Instance.GetForceThreeColumns())
        //{
        //    WrapGrid.ItemHeight = WrapGrid.ItemWidth = 185;
        //    WrapGrid.SizeChanged -= lst_SizeChanged;
        //    lst.SizeChanged -= lst_SizeChanged;
        //    return;
        //}
        //var width = e.NewSize.Width / 3f;
        //WrapGrid.ItemHeight = WrapGrid.ItemWidth = width;
    }

}
