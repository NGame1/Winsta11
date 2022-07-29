using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters.Directs
{
    public class HasUnreadToFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool val)
            {
                if (val) return FontWeights.SemiBold;
                return FontWeights.Normal;
            }
            return FontWeights.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
