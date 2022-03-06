using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Abstractions.Stories;

namespace WinstaNext.Models.TemplateSelectors.Stories
{
    public class WinstaStoryItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LiveTemplate { get; set; }
        public DataTemplate HashtagStoryTemplate { get; set; }
        public DataTemplate StoryTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is not WinstaStoryItem story) return null;

            if (story.Broadcast != null) return LiveTemplate;
            if (story.HashtagStory != null) return HashtagStoryTemplate;
            if (story.ReelFeed != null) return StoryTemplate;

            return base.SelectTemplateCore(item, container);
        }
    }
}
