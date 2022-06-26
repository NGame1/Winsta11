using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Attributes;
using WinstaNext.Core.Collections;
using WinstaNext.Core.Collections.IncrementalSources.Hashtags;
using WinstaNext.Core.Collections.IncrementalSources.Places;
using WinstaNext.Core.Collections.IncrementalSources.Users;
using WinstaNext.Core.Navigation;
using WinstaNext.Models.Core;
using WinstaNext.Services;
using WinstaNext.Views;
using WinstaNext.Views.Media;
using WinstaNext.Views.Profiles;

namespace WinstaNext.ViewModels.Users
{
    public class PlaceProfileViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }
        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }

        IncrementalPlaceTopMedias TopMediasInstance { get; set; }
        IncrementalPlaceRecentMedias RecentInstance { get; set; }

        RangePlayerAttribute TopMedias { get; set; }
        RangePlayerAttribute RecentMedias { get; set; }

        public RangePlayerAttribute ItemsSource { get; set; }

        public InstaPlaceShort Place { get; set; }

        public ScrollViewer ListViewScroll { get; set; }

        public RelayCommand<ItemClickEventArgs> NavigateToMediaCommand { get; set; }

        public ExtendedObservableCollection<MenuItemModel> ProfileTabs { get; set; } = new();

        [OnChangedMethod(nameof(ONSelectedTabChanged))]
        public MenuItemModel SelectedTab { get; set; }

        public PlaceProfileViewModel() : base()
        {
            NavigateToMediaCommand = new(NavigateToMedia);
        }

        void NavigateToMedia(ItemClickEventArgs args)
        {
            if (args.ClickedItem is not InstaMedia media) throw new ArgumentOutOfRangeException(nameof(args.ClickedItem));
            //var index = UserMedias.IndexOf(media);
            var para = new IncrementalMediaViewParameter(ItemsSource, media);
            NavigationService.Navigate(typeof(IncrementalInstaMediaView), para);
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            await Task.Delay(10);
            if (e.Parameter is string facebookPlaceId && !string.IsNullOrEmpty(facebookPlaceId))
            {
                if (Place != null && Place.FacebookPlacesId.ToString() == facebookPlaceId) return;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.LocationProcessor.GetLocationInfoAsync(facebookPlaceId);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    Place = result.Value;
                }
            }
            else if (e.Parameter is InstaPlaceShort placeShort)
            {
                if (Place != null && Place.FacebookPlacesId == placeShort.FacebookPlacesId) return;
                Place = placeShort;
            }
            else if (e.Parameter is InstaPlace place)
            {
                if (Place != null && Place.FacebookPlacesId == place.Location.FacebookPlacesId) return;
                Place = place.Location;
            }
            else if (e.Parameter is InstaLocation loc)
            {
                if (Place != null && Place.FacebookPlacesId == loc.FacebookPlacesId) return;
                Place = loc.Adapt<InstaPlaceShort>();
            }
            else
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                throw new ArgumentOutOfRangeException(nameof(e.Parameter));
            }
            RecentInstance = new(Place.FacebookPlacesId);
            TopMediasInstance = new(Place.FacebookPlacesId);
            RecentMedias = new(RecentInstance);
            TopMedias = new(TopMediasInstance);
            CreateProfileTabs();
            ListViewScroll.ChangeView(null, 0, null);
            await base.OnNavigatedToAsync(e);
        }

        void ONSelectedTabChanged()
        {
            if (SelectedTab == null) return;
            if (SelectedTab.Text == LanguageManager.Instance.Instagram.Top)
            {
                ItemsSource = TopMedias;
            }
            else if (SelectedTab.Text == LanguageManager.Instance.Instagram.Recent)
            {
                ItemsSource = RecentMedias;
            }
            else if (SelectedTab.Text == LanguageManager.Instance.Instagram.Reels)
            {
                //ItemsSource = TVMedias;
            }
        }

        void CreateProfileTabs()
        {
            SelectedTab = null;
            if (ProfileTabs.Any()) ProfileTabs.Clear();

            ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Recent, "\uE823"));

            ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Top, "\uE752"));

            ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Reels, "\uE102"));

            if (SelectedTab != null)
                SelectedTab = null;

            SelectedTab = ProfileTabs.FirstOrDefault();
        }

        private void UserProfileViewModel_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ViewHeight = e.NewSize.Height;
            ViewWidth = e.NewSize.Width;
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
            (NavigationService.Content as PlaceProfileView).SizeChanged += UserProfileViewModel_SizeChanged;
            base.OnNavigatedTo(e);
        }

        public override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            (NavigationService.Content as PlaceProfileView).SizeChanged -= UserProfileViewModel_SizeChanged;
            base.OnNavigatingFrom(e);
        }
    }
}
