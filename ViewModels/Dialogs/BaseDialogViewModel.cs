using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;

namespace ViewModels.Dialogs
{
    [AddINotifyPropertyChangedInterface]
    public class BaseDialogViewModel : ObservableObject
    {
        public Action HideDialogAction { get; set; }

        public BaseDialogViewModel()
        {

        }
    }
}
