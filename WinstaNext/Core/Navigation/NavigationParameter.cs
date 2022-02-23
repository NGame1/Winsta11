using WinstaNext.ViewModels;
using PropertyChanged;
using System.ComponentModel;

#nullable enable

namespace WinstaNext.Core.Navigation
{
    [AddINotifyPropertyChangedInterface]
    public class NavigationParameter
    {
        public NavigationParameter(object para = null)
        {
            Parameter = para;
        }

        public NavigationParameter(string query, object para = null)
        {
            Query = query;
            Parameter = para;
        }

        public string Query { get; } = "";
        
        public object Parameter { get; }

    }
}
