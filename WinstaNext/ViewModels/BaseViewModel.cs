using WinstaNext.Core.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;
using WinstaNext.Services;
using Microsoft.Extensions.DependencyInjection;
using WinstaNext.Models.Core;

namespace WinstaNext.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public abstract class BaseViewModel : ObservableRecipient
    {
        public bool IsLoading { get; set; }
        public abstract string PageHeader { get; protected set; }

        protected WinstaSynchronizationContext UIContext { get; }
        internal NavigationService NavigationService { get; private set; }

        public BaseViewModel()
        {
            UIContext = new(SynchronizationContext.Current);
            NavigationService = App.Container.GetService<NavigationService>();
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
