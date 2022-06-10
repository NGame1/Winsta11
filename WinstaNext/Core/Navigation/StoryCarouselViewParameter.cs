using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Core.Collections.IncrementalSources.Stories;

namespace WinstaNext.Core.Navigation
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
