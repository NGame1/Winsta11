using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TemplateSelectors
{
    public class SearchItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate PlaceTemplate { get; set; }
        public DataTemplate HashtagTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case InstaUser:
                    return UserTemplate;
                case InstaHashtag:
                    return HashtagTemplate;
                case InstaPlace:
                    return PlaceTemplate;
                default:
                    return null;
            }
        }
    }
}
