using System;
#nullable enable

namespace WinstaCore
{
    internal static class ServiceProviderExtensions
    {
        //
        // Summary:
        //     Get service of type T from the System.IServiceProvider.
        //
        // Parameters:
        //   provider:
        //     The System.IServiceProvider to retrieve the service object from.
        //
        // Type parameters:
        //   T:
        //     The type of service object to get.
        //
        // Returns:
        //     A service object of type T or null if there is no such service.
        public static T? GetService<T>(this IServiceProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            return (T)provider.GetService(typeof(T));
        }

    }
}
