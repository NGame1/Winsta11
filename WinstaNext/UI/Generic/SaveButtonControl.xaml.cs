using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Generic
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class SaveButtonControl : Button
    {
        public static readonly DependencyProperty GlyphProperty = DependencyProperty.Register(
                   nameof(Glyph),
                   typeof(string),
                   typeof(LikeButtonControl),
                   new PropertyMetadata(null));

        public static readonly DependencyProperty GlyphFontProperty = DependencyProperty.Register(
                nameof(GlyphFont),
                typeof(FontFamily),
                typeof(LikeButtonControl),
                new PropertyMetadata(null));

        //public static readonly DependencyProperty GlyphForegroundProperty = DependencyProperty.Register(
        //           nameof(GlyphForeground),
        //           typeof(SolidColorBrush),
        //           typeof(LikeButtonControl),
        //           new PropertyMetadata(null));

        public static readonly DependencyProperty IsSavedProperty = DependencyProperty.Register(
             nameof(IsSaved),
             typeof(bool),
             typeof(LikeButtonControl),
             new PropertyMetadata(null));

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }

        public FontFamily GlyphFont
        {
            get { return (FontFamily)GetValue(GlyphFontProperty); }
            set { SetValue(GlyphFontProperty, value); }
        }

        //public SolidColorBrush GlyphForeground
        //{
        //    get { return (SolidColorBrush)GetValue(GlyphForegroundProperty); }
        //    set { SetValue(GlyphForegroundProperty, value); }
        //}

        [OnChangedMethod(nameof(OnSavedChanged))]
        public bool IsSaved
        {
            get { return (bool)GetValue(IsSavedProperty); }
            set { SetValue(IsSavedProperty, value); }
        }

        public SaveButtonControl()
        {
            this.InitializeComponent();
            OnSavedChanged();
        }

        void OnSavedChanged()
        {
            if (!IsSaved)
            {
                GlyphFont = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];
                //GlyphForeground = (SolidColorBrush)App.Current.Resources.ThemeDictionaries["ApplicationForegroundThemeBrush"];
                Glyph = "\uF1F6";
            }
            else
            {
                GlyphFont = (FontFamily)App.Current.Resources["FluentSystemIconsFilled"];
                //GlyphForeground = new SolidColorBrush(Colors.Red);
                Glyph = "\uF1F6";
            }
        }

    }
}
