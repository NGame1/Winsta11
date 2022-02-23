using WinstaNext.Core.Navigation;
using WinstaNext.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinstaNext.Views
{
    public class BasePage : Page
    {
        public Thickness PageMargin { get => new Thickness(56, 12, 56, 0); }

        public BaseViewModel PageViewModel { get; private set; }

        public BasePage()
        {
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.DataContext is not BaseViewModel vm)
                throw new ArgumentException();
            PageViewModel = vm;
            try
            {
                PageViewModel.IsLoading = true;
                this.DataContext = PageViewModel;
                PageViewModel.OnNavigatedTo(e);
                await PageViewModel.OnNavigatedToAsync(e);
            }
            finally { PageViewModel.IsLoading = false; }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            PageViewModel.OnNavigatedFrom(e);
            await PageViewModel.OnNavigatedFromAsync(e);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            PageViewModel.IsLoading = true;
            try
            {
                PageViewModel.OnNavigatingFrom(e);
                await PageViewModel.OnNavigatingFromAsync(e);
            }
            finally { PageViewModel.IsLoading = false; }
        }

    }
}
