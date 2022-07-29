using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
#nullable enable

namespace Microsoft.Toolkit.Mvvm.Input
{
    /// <summary>
    /// A command that mirrors the functionality of <see cref="RelayCommand"/>, with the addition of
    /// accepting a <see cref="Func{TResult}"/> returning a <see cref="Task"/> as the execute
    /// action, and providing an <see cref="ExecutionTask"/> property that notifies changes when
    /// <see cref="ExecuteAsync"/> is invoked and when the returned <see cref="Task"/> completes.
    /// </summary>
    public sealed class AsyncRelayCommand : ObservableObject, IAsyncRelayCommand
    {
        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="CanBeCanceled"/>.
        /// </summary>
        internal static readonly PropertyChangedEventArgs CanBeCanceledChangedEventArgs = new(nameof(CanBeCanceled));

        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsCancellationRequested"/>.
        /// </summary>
        internal static readonly PropertyChangedEventArgs IsCancellationRequestedChangedEventArgs = new(nameof(IsCancellationRequested));

        /// <summary>
        /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="IsRunning"/>.
        /// </summary>
        internal static readonly PropertyChangedEventArgs IsRunningChangedEventArgs = new(nameof(IsRunning));

        /// <summary>
        /// The <see cref="Func{TResult}"/> to invoke when <see cref="Execute"/> is used.
        /// </summary>
        private readonly Func<Task>? execute;

        /// <summary>
        /// The cancelable <see cref="Func{T,TResult}"/> to invoke when <see cref="Execute"/> is used.
        /// </summary>
        /// <remarks>Only one between this and <see cref="execute"/> is not <see langword="null"/>.</remarks>
        private readonly Func<CancellationToken, Task>? cancelableExecute;

        /// <summary>
        /// The optional action to invoke when <see cref="CanExecute"/> is used.
        /// </summary>
        private readonly Func<bool>? canExecute;

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> instance to use to cancel <see cref="cancelableExecute"/>.
        /// </summary>
        /// <remarks>This is only used when <see cref="cancelableExecute"/> is not <see langword="null"/>.</remarks>
        private CancellationTokenSource? cancellationTokenSource;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public AsyncRelayCommand(Func<Task> execute)
        {
            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class that can always execute.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute)
        {
            this.cancelableExecute = cancelableExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public AsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
        {
            this.cancelableExecute = cancelableExecute;
            this.canExecute = canExecute;
        }

        private TaskNotifier? executionTask;

        /// <inheritdoc/>
        public Task? ExecutionTask
        {
            get => this.executionTask;
            private set
            {
                if (SetPropertyAndNotifyOnCompletion(ref this.executionTask, value, _ =>
                {
                    // When the task completes
                    OnPropertyChanged(IsRunningChangedEventArgs);
                    OnPropertyChanged(CanBeCanceledChangedEventArgs);
                }))
                {
                    // When setting the task
                    OnPropertyChanged(IsRunningChangedEventArgs);
                    OnPropertyChanged(CanBeCanceledChangedEventArgs);
                }
            }
        }

        /// <inheritdoc/>
        public bool CanBeCanceled => this.cancelableExecute is not null && IsRunning;

        /// <inheritdoc/>
        public bool IsCancellationRequested => this.cancellationTokenSource?.IsCancellationRequested == true;

        /// <inheritdoc/>
        public bool IsRunning => ExecutionTask?.IsCompleted == false;

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            return this.canExecute?.Invoke() != false;
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        /// <inheritdoc/>
        public Task ExecuteAsync(object? parameter)
        {
            if (CanExecute(parameter))
            {
                // Non cancelable command delegate
                if (this.execute is not null)
                {
                    return ExecutionTask = this.execute();
                }

                // Cancel the previous operation, if one is pending
                this.cancellationTokenSource?.Cancel();

                CancellationTokenSource cancellationTokenSource = this.cancellationTokenSource = new();

                OnPropertyChanged(IsCancellationRequestedChangedEventArgs);

                // Invoke the cancelable command delegate with a new linked token
                return ExecutionTask = this.cancelableExecute!(cancellationTokenSource.Token);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            this.cancellationTokenSource?.Cancel();

            OnPropertyChanged(IsCancellationRequestedChangedEventArgs);
            OnPropertyChanged(CanBeCanceledChangedEventArgs);
        }
    }

    /// <summary>
    /// A generic command that provides a more specific version of <see cref="AsyncRelayCommand"/>.
    /// </summary>
    /// <typeparam name="T">The type of parameter being passed as input to the callbacks.</typeparam>
    public sealed class AsyncRelayCommand<T> : ObservableObject, IAsyncRelayCommand<T>
    {
        /// <summary>
        /// The <see cref="Func{TResult}"/> to invoke when <see cref="Execute(T)"/> is used.
        /// </summary>
        private readonly Func<T?, Task>? execute;

        /// <summary>
        /// The cancelable <see cref="Func{T1,T2,TResult}"/> to invoke when <see cref="Execute(object?)"/> is used.
        /// </summary>
        private readonly Func<T?, CancellationToken, Task>? cancelableExecute;

        /// <summary>
        /// The optional action to invoke when <see cref="CanExecute(T)"/> is used.
        /// </summary>
        private readonly Predicate<T?>? canExecute;

        /// <summary>
        /// The <see cref="CancellationTokenSource"/> instance to use to cancel <see cref="cancelableExecute"/>.
        /// </summary>
        private CancellationTokenSource? cancellationTokenSource;

        /// <inheritdoc/>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
        public AsyncRelayCommand(Func<T?, Task> execute)
        {
            this.execute = execute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class that can always execute.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
        public AsyncRelayCommand(Func<T?, CancellationToken, Task> cancelableExecute)
        {
            this.cancelableExecute = cancelableExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
        public AsyncRelayCommand(Func<T?, Task> execute, Predicate<T?> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="cancelableExecute">The cancelable execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        /// <remarks>See notes in <see cref="RelayCommand{T}(Action{T})"/>.</remarks>
        public AsyncRelayCommand(Func<T?, CancellationToken, Task> cancelableExecute, Predicate<T?> canExecute)
        {
            this.cancelableExecute = cancelableExecute;
            this.canExecute = canExecute;
        }

        private TaskNotifier? executionTask;

        /// <inheritdoc/>
        public Task? ExecutionTask
        {
            get => this.executionTask;
            private set
            {
                if (SetPropertyAndNotifyOnCompletion(ref this.executionTask, value, _ =>
                {
                    // When the task completes
                    OnPropertyChanged(AsyncRelayCommand.IsRunningChangedEventArgs);
                    OnPropertyChanged(AsyncRelayCommand.CanBeCanceledChangedEventArgs);
                }))
                {
                    // When setting the task
                    OnPropertyChanged(AsyncRelayCommand.IsRunningChangedEventArgs);
                    OnPropertyChanged(AsyncRelayCommand.CanBeCanceledChangedEventArgs);
                }
            }
        }

        /// <inheritdoc/>
        public bool CanBeCanceled => this.cancelableExecute is not null && IsRunning;

        /// <inheritdoc/>
        public bool IsCancellationRequested => this.cancellationTokenSource?.IsCancellationRequested == true;

        /// <inheritdoc/>
        public bool IsRunning => ExecutionTask?.IsCompleted == false;

        /// <inheritdoc/>
        public void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(T? parameter)
        {
            return this.canExecute?.Invoke(parameter) != false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CanExecute(object? parameter)
        {
            if (default(T) is not null &&
                parameter is null)
            {
                return false;
            }

            return CanExecute((T?)parameter);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute(T? parameter)
        {
            _ = ExecuteAsync(parameter);
        }

        /// <inheritdoc/>
        public void Execute(object? parameter)
        {
            _ = ExecuteAsync((T?)parameter);
        }

        /// <inheritdoc/>
        public Task ExecuteAsync(T? parameter)
        {
            if (CanExecute(parameter))
            {
                // Non cancelable command delegate
                if (this.execute is not null)
                {
                    return ExecutionTask = this.execute(parameter);
                }

                // Cancel the previous operation, if one is pending
                this.cancellationTokenSource?.Cancel();

                CancellationTokenSource cancellationTokenSource = this.cancellationTokenSource = new();

                OnPropertyChanged(AsyncRelayCommand.IsCancellationRequestedChangedEventArgs);

                // Invoke the cancelable command delegate with a new linked token
                return ExecutionTask = this.cancelableExecute!(parameter, cancellationTokenSource.Token);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task ExecuteAsync(object? parameter)
        {
            return ExecuteAsync((T?)parameter);
        }

        /// <inheritdoc/>
        public void Cancel()
        {
            this.cancellationTokenSource?.Cancel();

            OnPropertyChanged(AsyncRelayCommand.IsCancellationRequestedChangedEventArgs);
            OnPropertyChanged(AsyncRelayCommand.CanBeCanceledChangedEventArgs);
        }
    }
}
