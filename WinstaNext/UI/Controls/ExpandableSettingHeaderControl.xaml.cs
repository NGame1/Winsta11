using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Controls
{
    [ContentProperty(Name = nameof(SettingActionableElement))]
    public sealed partial class ExpandableSettingHeaderControl : UserControl
    {
        public FrameworkElement? SettingActionableElement { get; set; }

        public static readonly DependencyProperty TitleProperty
           = DependencyProperty.Register(
               nameof(Title),
               typeof(string),
               typeof(ExpandableSettingHeaderControl),
               new PropertyMetadata(
                   string.Empty,
                   (d, e) =>
                   {
                       AutomationProperties.SetName(d, (string)e.NewValue);
                   }));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty
            = DependencyProperty.Register(
                nameof(Description),
                typeof(string),
                typeof(ExpandableSettingHeaderControl),
                new PropertyMetadata(
                    string.Empty,
                    (d, e) =>
                    {
                        AutomationProperties.SetHelpText(d, (string)e.NewValue);
                    }));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(
                nameof(Icon),
                typeof(IconElement),
                typeof(ExpandableSettingHeaderControl),
                new PropertyMetadata(null));

        public IconElement Icon
        {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public ExpandableSettingHeaderControl()
        {
            InitializeComponent();
            VisualStateManager.GoToState(this, "NormalState", false);
        }

        private void MainPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == e.PreviousSize.Width || ActionableElement == null)
            {
                return;
            }

            if (ActionableElement.ActualWidth > e.NewSize.Width / 3)
            {
                VisualStateManager.GoToState(this, "CompactState", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "NormalState", false);
            }
        }
    }
}
