using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;

namespace WinstaNext.ViewModels.Dialogs
{
    [AddINotifyPropertyChangedInterface]
    internal class BaseDialogViewModel : ObservableRecipient
    {
        internal Action HideDialogAction { get; set; }

        public BaseDialogViewModel()
        {

        }
    }
}
