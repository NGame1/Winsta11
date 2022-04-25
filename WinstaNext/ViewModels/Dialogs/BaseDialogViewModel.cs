using Microsoft.Toolkit.Mvvm.ComponentModel;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
