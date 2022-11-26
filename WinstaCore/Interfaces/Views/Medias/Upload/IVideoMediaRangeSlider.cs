using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace WinstaCore.Interfaces.Views.Medias.Upload
{
    public interface IVideoMediaRangeSlider
    {
        double Minimum { get; set; }
        double Maximum { get; set; }
        double RangeMin { get; set; }
        double RangeMax { get; set; }
        MediaElement MediaElement { get; set; }

        void AddImageToSlider(ImageSource src);

        event EventHandler MinimumValueChanged;
        event EventHandler MaximumValueChanged;
    }
}
