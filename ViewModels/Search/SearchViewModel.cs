﻿using Core.Collections;
using Core.Collections.IncrementalSources.Search;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
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
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;

namespace ViewModels.Search
{
    public class SearchViewModel : BaseViewModelWithStopwatch
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

        [OnChangedMethod(nameof(OnSearchQueryChanged))]
        public string SearchQuery { get; set; }

        [OnChangedMethod(nameof(OnSearchContextChanged))]
        public string SearchContext { get => SelectedItem == null ? "" : SelectedItem.Tag.ToString(); }

        IncrementalHashtagsSearch hashtagSearchinstance = new();
        IncrementalPlacesSearch placeSearchinstance = new();
        IncrementalPeopleSearch peopleSearchinstance = new();

        public SearchViewModel()
        {
            //MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Top) { Tag = "Top" });
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

        async void ListViewItemClick(ItemClickEventArgs e)
        {
            using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
            switch (e.ClickedItem)
            {
                case InstaUser user:
                    var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
                    NavigationService.Navigate(UserProfileView, user);
                    await Api.DiscoverProcessor.RegisterRecentSearchClickAsync(user.UserName, InstaSearchType.User, user.Pk.ToString());
                    break;

                case InstaHashtag hashtag:
                    var HashtagProfileView = AppCore.Container.GetService<IHashtagProfileView>();
                    NavigationService.Navigate(HashtagProfileView, hashtag);
                    await Api.DiscoverProcessor.RegisterRecentSearchClickAsync(hashtag.Name, InstaSearchType.Hashtag, hashtag.Id.ToString());
                    break;

                case InstaPlace place:
                    var PlaceProfileView = AppCore.Container.GetService<IPlaceProfileView>();
                    NavigationService.Navigate(PlaceProfileView, place);
                    await Api.DiscoverProcessor.RegisterRecentSearchClickAsync(place.Title, InstaSearchType.Place, place.Location.Pk.ToString());
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
                SelectedItem = MenuItems[0];
            }
            else SelectedItem = MenuItems.FirstOrDefault();
            await base.OnNavigatedToAsync(e);
        }

        void OnSearchQueryChanged() => OnSearchQueryChanged(false);
        async void OnSearchQueryChanged(bool contexChanged)
        {
            if (SearchQuery == string.Empty) return;
            if (!contexChanged && !await EnsureTimeElapsed()) return;
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
