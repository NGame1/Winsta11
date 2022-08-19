using Core.Collections.IncrementalSources.Directs;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using WinstaCore;

namespace ViewModels.Directs
{
    public class GiphyViewModel : BaseViewModel
    {
        IncrementalDirectGiphyCollection DirectInstance { get; }

        public IncrementalLoadingCollection<IncrementalDirectGiphyCollection, GiphyItem> DirectGiphyCollection { get; }

        public ISupportIncrementalLoading ItemsSource { get; set; }

        [OnChangedMethod(nameof(OnSearchQueryChanged))]
        public string SearchQuery { get; set; } = string.Empty;

        public GiphyViewModel()
        {
            DirectInstance = new();
            DirectGiphyCollection = new(DirectInstance);
            ItemsSource = DirectGiphyCollection;
        }

        public async Task SendGifAsync(GiphyItem gif, InstaDirectInboxThread thread)
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.SendDirectAnimatedMediaAsync(gif.Id, thread.ThreadId);
            }
        }

        void OnSearchQueryChanged()
        {
            if (ItemsSource == null) return;
            if (ItemsSource == DirectGiphyCollection)
            {
                DirectInstance.SearchQuery = SearchQuery;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
