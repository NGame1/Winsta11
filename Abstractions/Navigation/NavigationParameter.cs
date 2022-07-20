using PropertyChanged;

#nullable enable

namespace Abstractions.Navigation
{
    [AddINotifyPropertyChangedInterface]
    public class NavigationParameter
    {
        public NavigationParameter(object? para = null)
        {
            Parameter = para;
        }

        public NavigationParameter(string query, object? para = null)
        {
            Query = query;
            Parameter = para;
        }

        public string Query { get; } = string.Empty;
        
        public object? Parameter { get; }

    }
}
