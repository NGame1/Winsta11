using Abstractions.Navigation;
using Abstractions.Stories;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Attributes;
using WinstaCore.Interfaces.Views;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore.Services;
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

    public AsyncRelayCommand LoadMoreCommand { get; }

    private int _pagesLoaded = 0;

    public StaggeredHomeView()
    {
        this.InitializeComponent();
        LoadMoreCommand = new(LoadMoreAsync);
    }

    async Task LoadMoreMediaAsync()
    {
        await ViewModel.Medias.LoadMoreItemsAsync(1);
        _pagesLoaded++;
    }

    async Task LoadMoreAsync()
    {
        await LoadMoreMediaAsync();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.NavigationMode != NavigationMode.New) return;
        try
        {
            await LoadMoreMediaAsync();
            await LoadMoreMediaAsync();
            await LoadMoreMediaAsync();
            base.OnNavigatedTo(e);

            await Task.Delay(1000);
            RefreshContainer_RefreshRequested(null, null);
        }
        catch (Exception) { }
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
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
        if (scroll.VerticalOffset >= scroll.ScrollableHeight - 400
            && !LoadMoreCommand.IsRunning)
            await LoadMoreCommand.ExecuteAsync(null);
    }

    void FeedPostsList_ItemClick(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is not InstaMedia media) return;
        var NavigationService = App.Container.GetRequiredService<NavigationService>();
        var mediaView = App.Container.GetRequiredService<IIncrementalInstaMediaView>();
        NavigationService.Navigate(mediaView, new IncrementalMediaViewParameter(Medias, media));
    }

    private void StoriesList_ItemClick(object sender, ItemClickEventArgs e)
    {
        var stories = ViewModel.Stories;
        ViewModel.NavigationService.Navigate(typeof(StoryCarouselView), new StoryCarouselViewParameter((WinstaStoryItem)e.ClickedItem, ref stories));
    }

    private void RefreshContainer_RefreshRequested(Microsoft.UI.Xaml.Controls.RefreshContainer sender, Microsoft.UI.Xaml.Controls.RefreshRequestedEventArgs args)
    {
    }
}
