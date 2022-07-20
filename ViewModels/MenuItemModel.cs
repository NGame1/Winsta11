using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using PropertyChanged;
using Core.Collections;

namespace ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MenuItemModel
    {
        public MenuItemModel(string text)
        {
            Text = text;
        }
        public MenuItemModel(string text, Type view)
        {
            Text = text;
            View = view;
        }

        public MenuItemModel(string text, string glyph, Type view = null)
        {
            Icon = new FontIcon
            {
                Glyph = glyph,
                FontFamily = new("Segoe Fluent Icons")
                //FontFamily = (FontFamily)App.Current.Resources["FluentIcons"]
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
