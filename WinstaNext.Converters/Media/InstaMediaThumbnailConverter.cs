using InstagramApiSharp.Classes.Models;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;
using WinstaCore;
#nullable enable

namespace WinstaNext.Converters.Media
{
    public class InstaMediaThumbnailConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not InstaMedia media) return null;
            var Quality = ApplicationSettingsManager.Instance.GetPlaybackQuality();
            var QualityIndex = (int)Quality;
            switch (media.MediaType)
            {
                case InstaMediaType.Image:
                    {
                        if (media.Images.Count > QualityIndex)
                        {
                            return new Uri(media.Images[QualityIndex].Uri);
                        }
                        else return new Uri(media.Images.LastOrDefault().Uri);
                    }
                case InstaMediaType.Video:
                    {
                        if (media.Images.Count > QualityIndex)
                        {
                            return new Uri(media.Images[QualityIndex].Uri);
                        }
                        else return new Uri(media.Images.LastOrDefault().Uri);
                    }
                case InstaMediaType.Carousel:
                    {
                        if (media.Carousel[0].Images.Count > QualityIndex)
                        {
                            return new Uri(media.Carousel[0].Images[QualityIndex].Uri);
                        }
                        else return new Uri(media.Carousel[0].Images.LastOrDefault().Uri);
                    }
                default: throw new NotImplementedException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
