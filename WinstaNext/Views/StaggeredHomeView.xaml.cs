using Abstractions.Navigation;
using Abstractions.Stories;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Attributes;
using WinstaCore.Interfaces.Views;
using WinstaNext.Views.Stories;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class StaggeredHomeView : BasePage, IHomeView
{
    public override string PageHeader { get; protected set; }

    public RangePlayerAttribute Medias { get => ViewModel.Medias; }

    public StaggeredHomeView()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        await ViewModel.Medias.LoadMoreItemsAsync(1);
        await ViewModel.Medias.LoadMoreItemsAsync(1);
        await ViewModel.Medias.LoadMoreItemsAsync(1);
        base.OnNavigatedTo(e);
    }

    private void StoriesList_ItemClick(object sender, ItemClickEventArgs e)
    {
        var stories = ViewModel.Stories;
        ViewModel.NavigationService.Navigate(typeof(StoryCarouselView), new StoryCarouselViewParameter((WinstaStoryItem)e.ClickedItem, ref stories));
    }

    private void FeedPostsList_Loaded(object sender, RoutedEventArgs e)
    {
        var scroll = FeedPostsList.FindDescendantOrSelf<ScrollViewer>();
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

}
