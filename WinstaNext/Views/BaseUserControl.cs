using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.ViewModels;

namespace WinstaNext.Views
{
    public class BaseUserControl : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
          nameof(ViewModel),
          typeof(BaseViewModel),
          typeof(BaseUserControl),
          new PropertyMetadata(null));

        public BaseViewModel ViewModel
        {
            get { return (BaseViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public BaseUserControl()
        {

        }
    }
}
