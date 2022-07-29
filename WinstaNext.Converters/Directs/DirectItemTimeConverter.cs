using System;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters.Directs
{
    internal class DirectItemTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not DateTime time) return "";
            return time.ToLocalTime().ToString("t");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
