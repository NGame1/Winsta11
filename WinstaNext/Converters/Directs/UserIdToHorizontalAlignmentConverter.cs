using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters.Directs
{
    internal class UserIdToHorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var me = App.Container.GetService<InstaUserShort>();
            if (value is long val)
            {
                if (val == me.Pk) return HorizontalAlignment.Right;
                return HorizontalAlignment.Left;
            }
            return HorizontalAlignment.Center;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
