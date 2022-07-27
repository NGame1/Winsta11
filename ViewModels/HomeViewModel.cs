using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System.Threading.Tasks;
using WinstaCore.Attributes;
using Core.Collections.IncrementalSources.Stories;
using Core.Collections.IncrementalSources.Media;
using WinstaCore;
using Abstractions.Stories;

namespace ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public AsyncRelayCommand RefreshCommand { get; set; }

        public IncrementalLoadingCollection<IIncrementalSource<WinstaStoryItem>, WinstaStoryItem> Stories { get; }
        public RangePlayerAttribute Medias { get; }

        public IncrementalFeedStories FeedStories { get; } = new();
        public IncrementalHomeMedia FeedMedia { get; } = new(AppCore.IsDark);

        public HomeViewModel() : base()
        {
            Medias = new(FeedMedia);
            Stories = new(FeedStories);
            RefreshCommand = new(RefreshAsync);
        }

        private async Task RefreshAsync()
        {
            FeedMedia.RefreshRequested = true;
            FeedStories.RefreshRequested = true;
            await Stories.RefreshAsync();
            await Medias.RefreshAsync();
        }
    }
}
