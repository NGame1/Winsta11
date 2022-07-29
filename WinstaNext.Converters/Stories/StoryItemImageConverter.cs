using InstagramApiSharp.Classes.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace WinstaNext.Converters.Stories
{
    internal class StoryItemImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not InstaStoryItem storyItem || !storyItem.Images.Any())
                return new BitmapImage(new Uri("ms-appx:///Assets/Icons/WinstaLogo.png", UriKind.RelativeOrAbsolute));
            if (storyItem.Images.FirstOrDefault() is InstaImage image)
                return new BitmapImage(new Uri(image.Uri, UriKind.RelativeOrAbsolute));
            return new BitmapImage(new Uri("ms-appx:///Assets/Icons/WinstaLogo.png", UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
