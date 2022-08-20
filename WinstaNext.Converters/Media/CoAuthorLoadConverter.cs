using InstagramApiSharp.Classes.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Converters.Media
{
    public class CoAuthorLoadConverter : IValueConverter
    {
        public bool IsInverted { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not InstaMedia media) return false;
            if(!IsInverted)
            {
                if (media.CoAuthorsProducers != null && media.CoAuthorsProducers.Any())
                    return true;
                return false;
            }
            if (media.CoAuthorsProducers != null && media.CoAuthorsProducers.Any())
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
