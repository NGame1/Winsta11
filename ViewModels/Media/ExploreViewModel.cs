using Abstractions.Navigation;
using Core.Collections.IncrementalSources.Media;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Attributes;
using WinstaCore.Interfaces.Views.Medias;

namespace ViewModels.Media
{
    public class ExploreViewModel : BaseViewModel
    {
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
            var IncrementalInstaMediaView = AppCore.Container.GetService<IIncrementalInstaMediaView>();
            NavigationService.Navigate(IncrementalInstaMediaView, para);
        }
    }
}
