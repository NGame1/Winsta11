using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#nullable enable

namespace WinstaNext.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool val)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (IsInverted)
            {
                if (val)
                    return Visibility.Collapsed;
                return Visibility.Visible;
            }

            if (val)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
