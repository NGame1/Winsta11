using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinstaNext.Models.TemplateSelectors.Searches
{
    internal class SearchItemTemplateSelector : DataTemplateSelector
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
