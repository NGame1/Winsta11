using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Abstractions.Stories;

namespace WinstaNext.Models.TemplateSelectors.Searches
{
    internal class WinstaStoryFeedItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BroadcastTemplate { get; set; }
        public DataTemplate HashtagStoryTemplate { get; set; }
        public DataTemplate ReelFeedTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is not WinstaStoryItem storyItem) return null;

            if (storyItem.Broadcast != null) return BroadcastTemplate;
            if (storyItem.HashtagStory != null) return HashtagStoryTemplate;
            if (storyItem.ReelFeed != null) return ReelFeedTemplate;

            return null;
        }
    }
}
