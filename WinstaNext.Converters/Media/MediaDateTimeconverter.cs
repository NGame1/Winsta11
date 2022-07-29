using Resources;
using System;
using Windows.UI.Xaml.Data;
#nullable enable

namespace WinstaNext.Converters.Media
{
    public class MediaDateTimeconverter : IValueConverter
    {
        public bool ConvertToLocalTime { get; set; } = false;
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not DateTime dt) return null;
            if (ConvertToLocalTime)
                dt = dt.ToLocalTime();
            var sub = DateTime.Now.Subtract(dt);
            if ((int)sub.TotalSeconds < 60) return LanguageManager.Instance.Units.Recently;
            if ((int)sub.TotalMinutes == 1) return $"1 {LanguageManager.Instance.Units.MinuteAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)sub.TotalMinutes < 60) return $"{(int)sub.TotalMinutes} {LanguageManager.Instance.Units.MinutesAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)sub.TotalHours == 1) return $"1 {LanguageManager.Instance.Units.HourAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)sub.TotalHours < 24) return $"{(int)sub.TotalHours} {LanguageManager.Instance.Units.HoursAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)sub.TotalDays == 1) return $"1 {LanguageManager.Instance.Units.DayAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)sub.TotalDays < 30) return $"{(int)sub.TotalDays} {LanguageManager.Instance.Units.DaysAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)(sub.TotalDays / 30) == 1) return $"1 {LanguageManager.Instance.Units.MonthAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)(sub.TotalDays / 30) < 12) return $"{(int)(sub.TotalDays / 30)} {LanguageManager.Instance.Units.MonthsAbbreviation} {LanguageManager.Instance.Units.Ago}";
            if ((int)((sub.TotalDays / 30) / 12) == 1) return $"1 {LanguageManager.Instance.Units.YearAbbreviation} {LanguageManager.Instance.Units.Ago}";
            return $"{(int)((sub.TotalDays / 30) / 12)} {LanguageManager.Instance.Units.YearsAbbreviation} {LanguageManager.Instance.Units.Ago}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
