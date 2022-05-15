using InstagramApiSharp.Classes;
using Microsoft.Toolkit.Uwp;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using WinstaNext.Core.Collections.IncrementalSources.Directs;

namespace WinstaNext.ViewModels.Directs
{
    internal class GiphyViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

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

        void OnSearchQueryChanged()
        {
            if (ItemsSource == null) return;
            if(ItemsSource == DirectGiphyCollection)
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
