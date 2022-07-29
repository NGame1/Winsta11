using System;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters
{
    public class IntToIsEnabledConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not int val)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (IsInverted)
            {
                if (val == 0)
                    return true;
                return false;
            }

            if (val == 0)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
