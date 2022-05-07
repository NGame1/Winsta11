using System;
#nullable enable

namespace WinstaNext.Core.Exceptions
{
    public static class ArgumentNullExceptionExtensions
    {
        //
        // Summary:
        //     Throws an System.ArgumentNullException if argument is null.
        //
        // Parameters:
        //   argument:
        //     The reference type argument to validate as non-null.
        //
        //   paramName:
        //     The name of the parameter with which argument corresponds.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     argument is null.
        public static void ThrowIfNull(object? argument, string? paramName = null)
        {
            if (argument == null) throw new ArgumentNullException(nameof(argument));
        }

        public static T ThrowIfNull<T>(this T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
            return argument;
        }

    }
}
