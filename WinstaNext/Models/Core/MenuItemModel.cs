using WinstaNext.Core;
using WinstaNext.Core.Collections;
using WinstaNext.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using PropertyChanged;

namespace WinstaNext.Models.Core
{
    [AddINotifyPropertyChangedInterface]
    public class MenuItemModel
    {
        public MenuItemModel(string text)
        {
            Text = text;
        }

        public MenuItemModel(string text, IAnimatedVisualSource2 visual, Type view = null)
        {
            Icon = new AnimatedIcon() { Source = visual };
            Text = text;
            View = view;
        }

        public MenuItemModel(string text, string glyph, Type view = null)
        {
            Icon = new FontIcon
            {
                //FontSize = 32,
                Glyph = glyph,
                FontFamily = (FontFamily)App.Current.Resources["FluentIcons"]
            };
            Text = text;
            View = view;
        }

        public MenuItemModel(string text, Uri svguri, Type view = null)
        {
            Icon = new ImageIcon()
            {
                Source = new SvgImageSource(svguri) { RasterizePixelHeight = 200, RasterizePixelWidth = 200 }
            };
            Text = text;
            View = view;
        }

        public IconElement Icon { get; set; }

        public ExtendedObservableCollection<MenuItemModel> Items { get; } = new();

        public string Text { get; }

        public Type View { get; }

        public object Tag { get; set; }

        public string Badge { get; set; }
    }
}
