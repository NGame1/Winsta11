using Core.Collections;
using Core.Collections.IncrementalSources.Search;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using PropertyChanged;
using Resources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;

namespace ViewModels.Search
{
    public class SearchViewModel : BaseViewModel
    {
        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }

        public RelayCommand<ItemClickEventArgs> ListViewItemClickCommand { get; set; }
        public RelayCommand<SizeChangedEventArgs> ListViewSizeChangedCommand { get; set; }
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
            ListViewSizeChangedCommand = new(ListViewSizeChanged);

            HashtagsList.CollectionChanged += HashtagsList_CollectionChanged;
            PlacesList.CollectionChanged += PlacesList_CollectionChanged;
            UsersList.CollectionChanged += UsersList_CollectionChanged;
        }

        void ListViewItemClick(ItemClickEventArgs e)
        {
            switch (e.ClickedItem)
            {
                case InstaUser user:
                    var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
                    NavigationService.Navigate(UserProfileView, user);
                    break;

                case InstaHashtag hashtag:
                    var HashtagProfileView = AppCore.Container.GetService<IHashtagProfileView>();
                    NavigationService.Navigate(HashtagProfileView, hashtag);
                    break;

                case InstaPlace place:
                    var PlaceProfileView = AppCore.Container.GetService<IPlaceProfileView>();
                    NavigationService.Navigate(PlaceProfileView, place);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        void ListViewLoaded(object arg)
        {
            if (arg is not ListView lst) return;
            //var scroll = lst.FindAscendantOrSelf<ScrollViewer>();
            ViewHeight = lst.Height - 115;
            ViewWidth = lst.Width;
        }

        Size lastSize;
        private void ListViewSizeChanged(SizeChangedEventArgs e)
        {
            if (e.NewSize.Height <= 0 ) return;
            if (lastSize != null && ViewHeight == e.NewSize.Height) return;
            lastSize = e.NewSize;
            ViewHeight = e.NewSize.Height - 115;
            ViewWidth = e.NewSize.Width;
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
            if (e.NavigationMode == NavigationMode.Back) return;
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
            if (SearchQuery == string.Empty) return;
            if (!contexChanged)
            {
                StopTimer.Restart();
                await Task.Delay(500);
                if (StopTimer.ElapsedMilliseconds < 500) return;
            }
            if (SearchQuery == string.Empty) return;
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
