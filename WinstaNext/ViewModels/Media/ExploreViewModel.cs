using Core.Collections.IncrementalSources.Media;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Resources;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Attributes;
using WinstaNext.Core.Navigation;
using WinstaNext.Views.Media;
using ViewModels;

namespace WinstaNext.ViewModels.Media
{
    public class ExploreViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Explore;

        IncrementalExploreMedia ExploreInstance { get; set; }

        public RangePlayerAttribute ExploreMedias { get; set; }

        public RelayCommand<ItemClickEventArgs> NavigateToMediaCommand { get; set; }

        public ExploreViewModel()
        {
            ExploreInstance = new();
            ExploreMedias = new(ExploreInstance);
            NavigateToMediaCommand = new(NavigateToMedia);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ExploreMedias != null && ExploreMedias.Any()) return;
            base.OnNavigatedTo(e);
        }

        void NavigateToMedia(ItemClickEventArgs args)
        {
            if (args.ClickedItem is not InstaMedia media) throw new ArgumentOutOfRangeException(nameof(args.ClickedItem));
            var para = new IncrementalMediaViewParameter(ExploreMedias, media);
            NavigationService.Navigate(typeof(IncrementalInstaMediaView), para);
        }
    }
}
