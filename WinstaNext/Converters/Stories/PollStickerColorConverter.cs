using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using WinstaCore.Constants.Instagram.Stories;

namespace WinstaNext.Converters.Stories
{
    internal class PollStickerColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string color) return new SolidColorBrush(PollColorConstants.Black);
            var col = color.ToLower() switch
            {
                "black" => PollColorConstants.Black,
                "blue" => PollColorConstants.Blue,
                "green" => PollColorConstants.Green,
                "lavender" => PollColorConstants.Lavender,
                "orange" => PollColorConstants.Orange,
                "pink" => PollColorConstants.Pink,
                "purple" => PollColorConstants.Purple,
                _ => PollColorConstants.Black,
            };
            return new SolidColorBrush(col);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
