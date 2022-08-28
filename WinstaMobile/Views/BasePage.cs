using System;
using ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;

namespace WinstaMobile.Views
{
    public abstract class BasePage : Page
    {

        public abstract string PageHeader { get; protected set; }

        public Thickness PageMargin { get => new(0); }

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
