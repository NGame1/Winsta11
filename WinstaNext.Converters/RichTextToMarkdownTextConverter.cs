using System;
using Windows.UI.Xaml.Data;
using WinstaCore.Helpers;

namespace WinstaNext.Converters
{
    internal class RichTextToMarkdownTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string richtext) return string.Empty;
            if (string.IsNullOrEmpty(richtext)) return string.Empty;
            return richtext.RichTextToMarkdownText();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
