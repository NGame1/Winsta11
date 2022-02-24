using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Core.Collections.IncrementalSources.Media;
using WinstaNext.Core.Collections.IncrementalSources.Stories;

namespace WinstaNext.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public AsyncRelayCommand RefreshCommand { get; set; }

        public IncrementalLoadingCollection<IncrementalFeedStories, WinstaStoryItem> Stories { get; }
        public HomeMediaIncrementalLoadingCollection Medias { get; }

        public IncrementalFeedStories FeedStories { get; } = new();
        public IncrementalHomeMedia FeedMedia { get; } = new();

        public override string PageHeader { get; protected set; }

        public HomeViewModel() : base()
        {
            Medias = new(FeedMedia);
            Stories = new(FeedStories);
            RefreshCommand = new(RefreshAsync);
            Medias.FirstVisibleItemIndexChanged += Medias_FirstVisibleItemIndexChanged;
        }

        private void Medias_FirstVisibleItemIndexChanged(object sender, int e)
        {
            var playingItems = Medias.Where(x => x.Play);
            if(playingItems.Any())
            {
                for (int i = 0; i < playingItems.Count(); i++)
                {
                    playingItems.ElementAt(i).Play = false;
                }
            }
            Medias.ElementAt(e).Play = true;
        }

        private async Task RefreshAsync()
        {
            FeedMedia.RefreshRequested = true;
            FeedStories.RefreshRequested = true;
            await Stories.RefreshAsync();
            await Medias.RefreshAsync();
        }
    }

    public class HomeMediaIncrementalLoadingCollection : IncrementalLoadingCollection<IncrementalHomeMedia, InstaMedia>, IItemsRangeInfo
    {
        public HomeMediaIncrementalLoadingCollection(IncrementalHomeMedia source) : base(source)
        {

        }

        int FirstVisibleItemIndex { get; set; } = -1;
        int LastVisibleItemIndex { get; set; } = -1;

        public event EventHandler<int> FirstVisibleItemIndexChanged;
        public event EventHandler<int> LastVisibleItemIndexChanged;

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            if (visibleRange.FirstIndex == FirstVisibleItemIndex) return;
            var _tempfirst = FirstVisibleItemIndex;
            var _templast = LastVisibleItemIndex;
            FirstVisibleItemIndexChanged?.Invoke(this, visibleRange.FirstIndex);
            FirstVisibleItemIndex = visibleRange.FirstIndex;
            LastVisibleItemIndexChanged?.Invoke(this, visibleRange.LastIndex);
            LastVisibleItemIndex = visibleRange.LastIndex;
        }


        public void Dispose()
        {

        }
    }
}
