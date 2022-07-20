using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using PropertyChanged;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Models.Core;
using WinstaCore.Services;

namespace ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseViewModel : ObservableRecipient
    {
        public bool IsLoading { get; set; }
        public abstract string PageHeader { get; protected set; }

        protected WinstaSynchronizationContext UIContext { get; }
        public NavigationService NavigationService { get; private set; }

        public BaseViewModel()
        {
            UIContext = new(SynchronizationContext.Current);
            NavigationService = AppCore.Container.GetService<NavigationService>();
        }

        protected void SetHeader()
        {
            if (string.IsNullOrWhiteSpace(PageHeader))
                Messenger.Send(new ChangePageHeaderMessage(false, PageHeader));
            else Messenger.Send(new ChangePageHeaderMessage(true, PageHeader));
        }

        public virtual void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
        }

        public virtual Task OnNavigatedToAsync(NavigationEventArgs e) { return Task.CompletedTask; }

        public virtual void OnNavigatedFrom(NavigationEventArgs e) { }
        public virtual Task OnNavigatedFromAsync(NavigationEventArgs e) { return Task.CompletedTask; }

        public virtual void OnNavigatingFrom(NavigatingCancelEventArgs e) { }
        public virtual Task OnNavigatingFromAsync(NavigatingCancelEventArgs e) { return Task.CompletedTask; }


    }
}
