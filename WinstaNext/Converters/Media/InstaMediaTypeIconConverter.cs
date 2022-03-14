using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters.Media
{
    internal class InstaMediaTypeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not InstaMedia media) return string.Empty;
            if (!string.IsNullOrEmpty(media.ProductType) && media.ProductType.ToLower() == "igtv") return "\uFAF3";
            if (!string.IsNullOrEmpty(media.ProductType) && media.ProductType.ToLower() == "clips") return "\uF55A";
            switch (media.MediaType)
            {
                case InstaMediaType.Video: return "\uF84B";
                case InstaMediaType.Carousel: return "\uF78C";
                default:return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
