using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Core.Collections.IncrementalSources.Stories;

namespace WinstaNext.ViewModels.Stories
{
    internal class StoryCarouselViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

        public IncrementalLoadingCollection<IncrementalFeedStories, WinstaStoryItem> Stories { get; private set; }

        public StoryCarouselViewModel()
        {

        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            Stories = (IncrementalLoadingCollection<IncrementalFeedStories, WinstaStoryItem>)e.Parameter;
            base.OnNavigatedTo(e);
        }

    }
}
