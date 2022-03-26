using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Core.Attributes;
using WinstaNext.Core.Collections.IncrementalSources.Media;
using WinstaNext.Core.Collections.IncrementalSources.Stories;

namespace WinstaNext.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public AsyncRelayCommand RefreshCommand { get; set; }

        public IncrementalLoadingCollection<IncrementalFeedStories, WinstaStoryItem> Stories { get; }
        public RangePlayerAttribute Medias { get; }

        public IncrementalFeedStories FeedStories { get; } = new();
        public IncrementalHomeMedia FeedMedia { get; } = new();

        public override string PageHeader { get; protected set; }

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
