using InstagramApiSharp.Classes.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using WinstaCore;

namespace WinstaNext.Converters.Directs
{
    public class UserIdToProfilePictureVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var me = (InstaUserShort)AppCore.Container.GetService(typeof(InstaUserShort));
            if (value is long val)
            {
                if (val == me.Pk) return Visibility.Collapsed;
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
