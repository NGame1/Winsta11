using Resources;
using System;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters
{
    public class PossibleLargeNumbersConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not long val) return string.Empty;
            if (val < 1_000) return val.ToString();
            if (val < 1_000_000) return $"{(val / 1_000f).ToString("#,##0.0")} {LanguageManager.Instance.Units.Kilo}";
            if (val < 1_000_000_000) return $"{(val / 1_000_000f).ToString("#,##0.0")} {LanguageManager.Instance.Units.Million}";
            if (val < 1_000_000_000_000) return $"{(val / 1_000_000_000f).ToString("#,##0.0")} {LanguageManager.Instance.Units.Billion}";
            //1000000.ToString("N", CultureInfo.CreateSpecificCulture(language));
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
