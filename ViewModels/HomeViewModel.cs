using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System.Threading.Tasks;
using WinstaCore.Attributes;
using Core.Collections.IncrementalSources.Stories;
using Core.Collections.IncrementalSources.Media;
using WinstaCore;
using Abstractions.Stories;
using Windows.UI.Xaml.Controls;
using WinstaCore.Helpers.ExtensionMethods;
using System;

namespace ViewModels;

public class HomeViewModel : BaseViewModel
{
    public AsyncRelayCommand RefreshCommand { get; set; }
    public AsyncRelayCommand<ListView> GoTopCommand { get; set; }

    public IncrementalLoadingCollection<IIncrementalSource<WinstaStoryItem>, WinstaStoryItem> Stories { get; }
    public RangePlayerAttribute Medias { get; }

    public IncrementalFeedStories FeedStories { get; } = new();
    public IncrementalHomeMedia FeedMedia { get; } = new(AppCore.IsDark);

    public HomeViewModel() : base()
    {
        Medias = new(FeedMedia);
        Stories = new(FeedStories);
        Medias.OnError = ErrorHandler;
        Stories.OnError = ErrorHandler;
        RefreshCommand = new(RefreshAsync);
        GoTopCommand = new(GoToTopAsync);
    }

    void ErrorHandler(Exception exception)
    {
        
    }

    async Task GoToTopAsync(ListView list)
    {
        if (list == null) return;
        await list.SmoothScrollIntoViewWithIndexAsync(0);
    }

    public virtual async Task RefreshAsync()
    {
        FeedMedia.RefreshRequested = true;
        FeedStories.RefreshRequested = true;
        await Stories.RefreshAsync();
        await Medias.RefreshAsync();
        await Stories.LoadMoreItemsAsync(1);
    }
}
