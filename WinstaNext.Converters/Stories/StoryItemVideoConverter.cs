using InstagramApiSharp.Classes.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters.Stories
{
    internal class StoryItemVideoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not InstaStoryItem storyItem || !storyItem.Images.Any())
                return new Uri("ms-appx:///Assets/Icons/WinstaLogo.png", UriKind.RelativeOrAbsolute);
            if (storyItem.Videos.FirstOrDefault() is InstaVideo video)
                return new Uri(video.Uri, UriKind.RelativeOrAbsolute);
            return new Uri("ms-appx:///Assets/Icons/WinstaLogo.png", UriKind.RelativeOrAbsolute);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
