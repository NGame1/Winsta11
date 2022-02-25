using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace WinstaNext.Converters.Stories
{
    internal class StoryBorderColorConverter : IValueConverter
    {
        public GradientBrush CloseFriends { get; set; }
        public GradientBrush NormalStory { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is InstaReelFeed story)
            {
                if (story.LatestReelMedia == story.Seen)
                    return null;
                if (story.HasBestiesMedia)
                    return CloseFriends;
                return NormalStory;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
