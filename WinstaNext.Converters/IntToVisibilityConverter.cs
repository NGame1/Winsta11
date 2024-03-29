﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters
{
    public class IntToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not int val)
                throw new ArgumentOutOfRangeException(nameof(value));

            if (IsInverted)
            {
                if (val == 0)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }

            if (val == 0)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
