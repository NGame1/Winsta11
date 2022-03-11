using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.UI.Xaml.Controls;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Collections;
using WinstaNext.Core.Collections.IncrementalSources.Search;
using WinstaNext.Models.Core;
using WinstaNext.Views.Profiles;

namespace WinstaNext.ViewModels.Search
{
    public class SearchViewModel : BaseViewModel
    {
        public RelayCommand<ItemClickEventArgs> ListViewItemClickCommand { get; set; }
        public RelayCommand<object> ListViewLoadedCommand { get; set; }

        public IncrementalLoadingCollection<IncrementalHashtagsSearch, InstaHashtag> HashtagsList { get; set; }
        public IncrementalLoadingCollection<IncrementalPlacesSearch, InstaPlace> PlacesList { get; set; }
        public IncrementalLoadingCollection<IncrementalPeopleSearch, InstaUser> UsersList { get; set; }
        public ExtendedObservableCollection<object> SearchResults { get; } = new();
        public List<MenuItemModel> MenuItems { get; } = new();

        [AlsoNotifyFor(nameof(SearchContext))]
        public MenuItemModel SelectedItem { get; set; }

        public override string PageHeader { get; protected set; } = LanguageManager.Instance.General.Search;

        [OnChangedMethod(nameof(OnSearchQueryChanged))]
        public string SearchQuery { get; set; }

        [OnChangedMethod(nameof(OnSearchContextChanged))]
        public string SearchContext { get => SelectedItem == null ? "" : SelectedItem.Tag.ToString(); }

        IncrementalHashtagsSearch hashtagSearchinstance = new();
        IncrementalPlacesSearch placeSearchinstance = new();
        IncrementalPeopleSearch peopleSearchinstance = new();

        public SearchViewModel()
        {
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Top) { Tag = "Top" });
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Accounts) { Tag = "Accounts" });
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Places) { Tag = "Places" });
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Hashtags) { Tag = "Hashtags" });

            HashtagsList = new(hashtagSearchinstance);
            PlacesList = new(placeSearchinstance);
            UsersList = new(peopleSearchinstance);

            ListViewLoadedCommand = new(ListViewLoaded);
            ListViewItemClickCommand = new(ListViewItemClick);

            HashtagsList.CollectionChanged += HashtagsList_CollectionChanged;
            PlacesList.CollectionChanged += PlacesList_CollectionChanged;
            UsersList.CollectionChanged += UsersList_CollectionChanged;
        }

        void ListViewItemClick(ItemClickEventArgs e)
        {
            switch (e.ClickedItem)
            {
                case InstaUser user:
                    NavigationService.Navigate(typeof(UserProfileView), user);
                    break;

                case InstaHashtag hashtag:

                    throw new NotImplementedException();
                    break;

                case InstaPlace place:
                    throw new NotImplementedException();
                    break;

                default:
                    break;
            }
        }

        void ListViewLoaded(object arg)
        {
            if (arg is not ListView lst) return;
            var scroll = lst.FindAscendantOrSelf<ScrollViewer>();
            scroll.ViewChanging += Scroll_ViewChanging;
        }

        private void Scroll_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {

        }

        private void HashtagsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SearchContext.ToLower() != "hashtags" ||
                      e.NewItems == null)
                return;

            SearchResults.AddRange(e.NewItems.Cast<object>());
        }

        private void PlacesList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SearchContext.ToLower() != "places" ||
                   e.NewItems == null)
                return;

            SearchResults.AddRange(e.NewItems.Cast<object>());
        }

        private void UsersList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SearchContext.ToLower() != "accounts" ||
                e.NewItems == null)
                return;

            SearchResults.AddRange(e.NewItems.Cast<object>());

        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            if (e.Parameter != null && !string.IsNullOrEmpty(e.Parameter.ToString()))
            {
                SearchQuery = e.Parameter.ToString();
                await Task.Delay(10);
                SelectedItem = MenuItems[1];
            }
            else SelectedItem = MenuItems.FirstOrDefault();
            await base.OnNavigatedToAsync(e);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            StopTimer = Stopwatch.StartNew();
            base.OnNavigatedTo(e);
        }

        public override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            StopTimer.Stop();
            StopTimer = null;
            base.OnNavigatingFrom(e);
        }

        Stopwatch StopTimer { get; set; }
        void OnSearchQueryChanged() => OnSearchQueryChanged(false);
        async void OnSearchQueryChanged(bool contexChanged)
        {
            if (!contexChanged)
            {
                StopTimer.Restart();
                await Task.Delay(400);
                if (StopTimer.ElapsedMilliseconds < 400) return;
            }
            SearchResults.Clear();
            switch (SearchContext.ToLower())
            {
                case "top":
                    break;

                case "accounts":
                    {
                        peopleSearchinstance.SearchQuerry = SearchQuery;
                        await UsersList.LoadMoreItemsAsync(1);
                    }
                    break;
                case "places":
                    {
                        placeSearchinstance.SearchQuerry = SearchQuery;
                        await PlacesList.LoadMoreItemsAsync(1);
                    }
                    break;
                case "hashtags":
                    {
                        hashtagSearchinstance.SearchQuerry = SearchQuery;
                        await HashtagsList.LoadMoreItemsAsync(1);
                    }
                    break;
                default:
                    break;
            }
        }

        void OnSearchContextChanged()
        {
            SearchResults.Clear();
            switch (SearchContext.ToLower())
            {
                case "top":
                    break;

                case "accounts":
                    SearchResults.AddRange(UsersList);
                    break;
                case "places":
                    SearchResults.AddRange(PlacesList);
                    break;
                case "hashtags":
                    SearchResults.AddRange(HashtagsList);
                    break;
                default:
                    break;
            }
            if (!SearchResults.Any())
                OnSearchQueryChanged(true);
        }

    }
}
