using WinstaNext.ViewModels;
using System;

namespace WinstaNext.Core.Messages
{
    public sealed class NavigateToPageMessage
    {
        public object Parameter { get; }

        public Type View { get; }
        //public BaseViewModel ViewModel { get; }

        public NavigateToPageMessage(Type view, object para = null)
        {
            View = view;
            Parameter = para;
        }

    }
}
