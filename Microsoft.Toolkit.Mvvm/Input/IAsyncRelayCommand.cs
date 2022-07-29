using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
#nullable enable

namespace Microsoft.Toolkit.Mvvm.Input
{
    //
    // Summary:
    //     An interface expanding Microsoft.Toolkit.Mvvm.Input.IRelayCommand to support
    //     asynchronous operations.
    public interface IAsyncRelayCommand : IRelayCommand, ICommand, INotifyPropertyChanged
    {
        //
        // Summary:
        //     Gets the last scheduled System.Threading.Tasks.Task, if available. This property
        //     notifies a change when the System.Threading.Tasks.Task completes.
        Task? ExecutionTask { get; }

        //
        // Summary:
        //     Gets a value indicating whether running operations for this command can be canceled.
        bool CanBeCanceled { get; }

        //
        // Summary:
        //     Gets a value indicating whether a cancelation request has been issued for the
        //     current operation.
        bool IsCancellationRequested { get; }

        //
        // Summary:
        //     Gets a value indicating whether the command currently has a pending operation
        //     being executed.
        bool IsRunning { get; }

        //
        // Summary:
        //     Provides a more specific version of System.Windows.Input.ICommand.Execute(System.Object),
        //     also returning the System.Threading.Tasks.Task representing the async operation
        //     being executed.
        //
        // Parameters:
        //   parameter:
        //     The input parameter.
        //
        // Returns:
        //     The System.Threading.Tasks.Task representing the async operation being executed.
        Task ExecuteAsync(object? parameter);

        //
        // Summary:
        //     Communicates a request for cancelation.
        //
        // Remarks:
        //     If the underlying command is not running, or if it does not support cancelation,
        //     this method will perform no action. Note that even with a successful cancelation,
        //     the completion of the current operation might not be immediate.
        void Cancel();
    }

    //
    // Summary:
    //     A generic interface representing a more specific version of Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.
    //
    // Type parameters:
    //   T:
    //     The type used as argument for the interface methods.
    //
    // Remarks:
    //     This interface is needed to solve the diamond problem with base classes.
    public interface IAsyncRelayCommand<T> : IAsyncRelayCommand, IRelayCommand, ICommand, INotifyPropertyChanged, IRelayCommand<T>
    {
        //
        // Summary:
        //     Provides a strongly-typed variant of Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand.ExecuteAsync(System.Object).
        //
        // Parameters:
        //   parameter:
        //     The input parameter.
        //
        // Returns:
        //     The System.Threading.Tasks.Task representing the async operation being executed.
        Task ExecuteAsync(T? parameter);
    }
}
