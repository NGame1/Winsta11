using InstagramApiSharp.Enums;
using Resources;
using System;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters.Enums;

public class InstaFollowOrderTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not InstaFollowOrderType val) return string.Empty;
        return val switch
        {
            InstaFollowOrderType.DateFollowedEarliest => LanguageManager.Instance.General.EarliestFirst,
            InstaFollowOrderType.DateFollowedLatest => LanguageManager.Instance.General.LatestFirst,
            InstaFollowOrderType.Default => LanguageManager.Instance.General.Default,
            _ => string.Empty,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
