using Microsoft.Toolkit.Uwp.UI;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Interfaces.Views.Medias;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Media;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class IncrementalStaggeredView : BasePage, IIncrementalInstaMediaView
{
    public override string PageHeader { get; protected set; } = string.Empty;

    public IncrementalStaggeredView()
    {
        this.InitializeComponent();
    }

    private void MediaPostsList_ItemClick(object sender, ItemClickEventArgs e)
    {

    }

    private void MediaPostsList_Loaded(object sender, RoutedEventArgs e)
    {
        var scroll = MediaPostsList.FindDescendantOrSelf<ScrollViewer>();
        if (scroll == null) return;
        scroll.ViewChanged += Scroll_ViewChanged;
    }

    async void Scroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        var scroll = (ScrollViewer)sender;
        if (scroll.VerticalOffset >= scroll.ScrollableHeight - 400
            && !ViewModel.MediaSource.IsLoading)
            await ViewModel.MediaSource.LoadMoreItemsAsync(1);
    }
}
