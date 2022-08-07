using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
#nullable enable

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Controls
{
    [ContentProperty(Name = nameof(SettingActionableElement))]
    public sealed partial class NavigationSettingContent : UserControl
    {
        public FrameworkElement? SettingActionableElement { get; set; }

        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(ExpandableSettingControl),
                new PropertyMetadata(string.Empty));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty TitleProperty
            = DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(ExpandableSettingControl),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty
            = DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(ExpandableSettingControl),
                new PropertyMetadata(string.Empty));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(
                nameof(Icon),
                typeof(IconElement),
                typeof(ExpandableSettingControl),
                new PropertyMetadata(null));

        public IconElement Icon
        {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public NavigationSettingContent()
        {
            this.InitializeComponent();
        }
    }
}
