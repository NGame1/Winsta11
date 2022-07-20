using System;

namespace WinstaCore.Models.Core
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
