using System;

namespace ViewModels
{
    internal static class ServiceProviderExtensions
    {
        internal static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
    }
}
