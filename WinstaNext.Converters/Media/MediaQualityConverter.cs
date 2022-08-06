using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;
using WinstaCore;

namespace WinstaNext.Converters.Media
{
    public class MediaQualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var Quality = ApplicationSettingsManager.Instance.GetPlaybackQuality();
            var QualityIndex = (int)Quality;
            switch (value)
            {
                    case List<InstaImage> Images:
                    {
                        if (Images.Count > QualityIndex)
                        {
                            return new Uri(Images[QualityIndex].Uri);
                        }
                        else return new Uri(Images.LastOrDefault().Uri);
                    }

                case List<InstaVideo> Videos:
                    {
                        if (Videos.Count > QualityIndex)
                        {
                            return new Uri(Videos[QualityIndex].Uri);
                        }
                        else return new Uri(Videos.LastOrDefault().Uri);
                    }

                default:
                    return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
