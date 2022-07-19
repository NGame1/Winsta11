using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;

namespace ViewModels.Dialogs
{
    [AddINotifyPropertyChangedInterface]
    public class BaseDialogViewModel : ObservableRecipient
    {
        public Action HideDialogAction { get; set; }

        public BaseDialogViewModel()
        {

        }
    }
}
