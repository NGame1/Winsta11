using Abstractions.Stories;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TemplateSelectors
{
    public class WinstaStoryFeedItemTemplateSelector : DataTemplateSelector
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
