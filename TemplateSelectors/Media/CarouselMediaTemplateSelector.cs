using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TemplateSelectors
{
    public class CarouselMediaTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ImageTemplate { get; set; }
        public DataTemplate VideoTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is not InstaCarouselItem carouselItem) return null;
            switch (carouselItem.MediaType)
            {
                case InstaMediaType.Image:return ImageTemplate;
                case InstaMediaType.Video:return VideoTemplate;
                default:return null;
            }
        }
    }
}
