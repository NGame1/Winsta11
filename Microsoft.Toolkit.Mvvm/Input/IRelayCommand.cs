using System.Windows.Input;
#nullable enable

namespace Microsoft.Toolkit.Mvvm.Input
{
    //
    // Summary:
    //     An interface expanding System.Windows.Input.ICommand with the ability to raise
    //     the System.Windows.Input.ICommand.CanExecuteChanged event externally.
    public interface IRelayCommand : ICommand
    {
        //
        // Summary:
        //     Notifies that the System.Windows.Input.ICommand.CanExecute(System.Object) property
        //     has changed.
        void NotifyCanExecuteChanged();
    }

    //
    // Summary:
    //     A generic interface representing a more specific version of Microsoft.Toolkit.Mvvm.Input.IRelayCommand.
    //
    // Type parameters:
    //   T:
    //     The type used as argument for the interface methods.
    public interface IRelayCommand<in T> : IRelayCommand, ICommand
    {
        //
        // Summary:
        //     Provides a strongly-typed variant of System.Windows.Input.ICommand.CanExecute(System.Object).
        //
        // Parameters:
        //   parameter:
        //     The input parameter.
        //
        // Returns:
        //     Whether or not the current command can be executed.
        //
        // Remarks:
        //     Use this overload to avoid boxing, if T is a value type.
        bool CanExecute(T? parameter);

        //
        // Summary:
        //     Provides a strongly-typed variant of System.Windows.Input.ICommand.Execute(System.Object).
        //
        // Parameters:
        //   parameter:
        //     The input parameter.
        //
        // Remarks:
        //     Use this overload to avoid boxing, if T is a value type.
        void Execute(T? parameter);
    }
}
