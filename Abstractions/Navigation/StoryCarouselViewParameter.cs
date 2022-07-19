using Abstractions.Stories;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;

namespace Abstractions.Navigation
{
    public class StoryCarouselViewParameter
    {
        public StoryCarouselViewParameter(WinstaStoryItem target)
        {
            TargetItem = target;
            Stories = null;
        }

        public StoryCarouselViewParameter(WinstaStoryItem target, ref IncrementalLoadingCollection<IIncrementalSource<WinstaStoryItem>, WinstaStoryItem> stories)
        {
            TargetItem = target;
            Stories = stories;
        }

        public WinstaStoryItem TargetItem { get; }
        public IncrementalLoadingCollection<IIncrementalSource<WinstaStoryItem>, WinstaStoryItem>? Stories { get; }
    }
}
