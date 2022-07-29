using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using WinstaCore.Helpers;

namespace WinstaNext.Converters
{
    public class TextToFlowDirectionConverter : IValueConverter
    {
        public bool IsInverted { get; set; } = false;
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string str) return FlowDirection.LeftToRight;
            if(!IsInverted)
            {
                if (str.IsRightToLeft()) return FlowDirection.RightToLeft;
                else return FlowDirection.LeftToRight;
            }
            else
            {
                if (str.IsRightToLeft()) return FlowDirection.LeftToRight;
                else return FlowDirection.RightToLeft;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
