using Abstractions.Navigation;
using Abstractions.Stories;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Attributes;
using WinstaCore.Interfaces.Views;
using WinstaNext.UI.Media;
using WinstaNext.Views.Stories;
#nullable enable

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StaggeredHomeView : BasePage, IHomeView
{
    public override string PageHeader { get; protected set; } = string.Empty;

    public RangePlayerAttribute Medias { get => ViewModel.Medias; }

    public StaggeredHomeView()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.NavigationMode != NavigationMode.New) return;
        await ViewModel.Medias.LoadMoreItemsAsync(1);
        await ViewModel.Medias.LoadMoreItemsAsync(1);
        await ViewModel.Medias.LoadMoreItemsAsync(1);
        base.OnNavigatedTo(e);
    }

    private void FeedPostsList_Loaded(object sender, RoutedEventArgs e)
    {
        var scroll = FeedPostsList.FindDescendantOrSelf<ScrollViewer>();
        if (scroll == null) return;
        scroll.ViewChanged += Scroll_ViewChanged;
    }

    async void Scroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
    {
        var scroll = (ScrollViewer)sender;
        if (scroll.VerticalOffset == scroll.ScrollableHeight)
        {
            await ViewModel.Medias.LoadMoreItemsAsync(1);
        }
    }

    Stopwatch? Stopwatch { get; set; }
    int TapsCount { get; set; } = 0;
    async void FeedPostsList_ItemClick(object sender, ItemClickEventArgs e)
    {
        Stopwatch ??= Stopwatch.StartNew();
        Stopwatch.Restart();
        TapsCount++;
        await Task.Delay(300);
        if (Stopwatch.ElapsedMilliseconds < 300) return;
        if (e.ClickedItem is not InstaMedia media) return;
        var container = (ListViewItem)FeedPostsList.ContainerFromItem(media);
        if (container.ContentTemplateRoot is not StaggeredTileUC tile) return;

        if (TapsCount >= 2)
        {
            DisposeStopwatch();
            await tile.LikeMediaCommand.ExecuteAsync(null);
            return;
        }
        else
        {
            DisposeStopwatch();
            tile.NavigateToMedia(Medias);
        }
    }

    private void FeedPostsList_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        TapsCount = 2;
    }

    void DisposeStopwatch()
    {
        Stopwatch?.Stop();
        Stopwatch = null;
        TapsCount = 0;
    }
}
