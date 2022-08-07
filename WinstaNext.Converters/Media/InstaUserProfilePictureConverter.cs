using InstagramApiSharp.Classes.Models;
using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace WinstaNext.Converters.Media
{
    public class InstaUserProfilePictureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return new BitmapImage(new Uri("ms-appx:///Assets/Icons/NoOne.png"));
            var uri = value switch
            {
                InstaUser instaUser => !instaUser.HasAnonymousProfilePicture || (instaUser.ProfilePictureId != null && instaUser.ProfilePictureId != "unknown") ?
                          instaUser.ProfilePicUrl : "ms-appx:///Assets/Icons/NoOne.png",

                InstaUserShort instaUserShort => !instaUserShort.HasAnonymousProfilePicture || (instaUserShort.ProfilePictureId != null && instaUserShort.ProfilePictureId != "unknown") ?
                               instaUserShort.ProfilePicUrl : "ms-appx:///Assets/Icons/NoOne.png",

                InstaUserInfo instaUserInfo => !instaUserInfo.HasAnonymousProfilePicture || (instaUserInfo.ProfilePictureId != null && instaUserInfo.ProfilePictureId != "unknown") ?
                               instaUserInfo.ProfilePicUrl : "ms-appx:///Assets/Icons/NoOne.png",

                _ => "ms-appx:///Assets/Icons/NoOne.png"

            };
            if(string.IsNullOrEmpty(uri)) return new BitmapImage(new Uri("ms-appx:///Assets/Icons/NoOne.png"));
            return new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
