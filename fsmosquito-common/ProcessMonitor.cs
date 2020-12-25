namespace FsMosquito
{
    using CliWrap.EventStream;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an object that monitors the indicated process.
    /// </summary>
    public class ProcessMonitor : IObservable<CommandEvent>, IDisposable
    {
        private readonly ConcurrentDictionary<IObserver<CommandEvent>, CommandEventUnsubscriber> _observers = new ConcurrentDictionary<IObserver<CommandEvent>, CommandEventUnsubscriber>();
        private readonly ProcessObserver _observer;
        private readonly ILogger<ProcessMonitor> _logger;

        private IDisposable _simConnectSubscription;
        private bool _isDisposed;

        public ProcessMonitor(IProcess process, ILogger<ProcessMonitor> logger)
        {
            Process = process ?? throw new ArgumentNullException(nameof(process));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _observer = new ProcessObserver(this);
        }

        IDisposable IObservable<CommandEvent>.Subscribe(IObserver<CommandEvent> observer)
        {
            return _observers.GetOrAdd(observer, new CommandEventUnsubscriber(this, observer));
        }

        /// <summary>
        /// Gets the process configuration
        /// </summary>
        public IProcess Process
        {
            get;
        }

        /// <summary>
        /// Starts the process and begins monitoring it 
        /// </summary>
        public void Start()
        {
            // If we already have a subscription, dispose of it.
            if (_simConnectSubscription != null)
            {
                _simConnectSubscription.Dispose();
                _simConnectSubscription = null;
            }

            _simConnectSubscription = Process.Command.Observe().Subscribe(_observer);
        }

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_simConnectSubscription != null)
                    {
                        _simConnectSubscription.Dispose();
                        _simConnectSubscription = null;
                    }
                }
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Nested Classes
        private sealed class ProcessObserver : IObserver<CommandEvent>
        {
            public ProcessObserver(ProcessMonitor parent)
            {
                Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            }

            public ProcessMonitor Parent { get; }

            public void OnCompleted()
            {
                if (Parent._logger != null)
                {
                    Parent._logger.LogInformation($"{Parent.Process.Name} Completed.");
                }

                // Chain the event
                Parallel.ForEach(Parent._observers.Keys, (observer) =>
                {
                    try
                    {
                        observer.OnCompleted();
                    }
                    catch (Exception)
                    {
                        // Do Nothing.
                    }
                });
            }

            public void OnError(Exception error)
            {
                if (Parent._logger != null)
                {
                    Parent._logger.LogError($"An exception occurred during monitoring of {Parent.Process.Name}: {error.Message}", error);
                }

                // Chain the event
                Parallel.ForEach(Parent._observers.Keys, (observer) =>
                {
                    try
                    {
                        observer.OnError(error);
                    }
                    catch (Exception)
                    {
                        // Do Nothing.
                    }
                });

                // Restart the shim if we didn't get a clean exit.
                Task.Run(() => Parent.Start());
            }

            public void OnNext(CommandEvent cmdEvent)
            {
                // Chain the event
                Parallel.ForEach(Parent._observers.Keys, (observer) =>
                {
                    try
                    {
                        observer.OnNext(cmdEvent);
                    }
                    catch (Exception)
                    {
                        // Do Nothing.
                    }
                });

                switch (cmdEvent)
                {
                    case StartedCommandEvent started:
                        if (Parent._logger != null)
                        {
                            Parent._logger.LogInformation($"{Parent.Process.Name} Started. (ProcessId {started.ProcessId})");
                        }

                        break;
                    case StandardOutputCommandEvent stdOut:
                        break;
                    case StandardErrorCommandEvent stdErr:
                        break;
                    case ExitedCommandEvent exited:
                        if (Parent._logger != null)
                        {
                            Parent._logger.LogWarning($"{Parent.Process.Name} Exited: {exited.ExitCode}");
                        }

                        // Restart the shim if we didn't get a clean exit.
                        if (exited.ExitCode != 0)
                        {
                            Task.Run(() => Parent.Start());
                        }
                        break;
                    default:
                        if (Parent._logger != null)
                        {
                            Parent._logger.LogWarning($"Internal Error while monitoring {Parent.Process.Name} - Unknown or unsupported CommandEvent: {cmdEvent}");
                        }
                        break;
                }
            }
        }

        private sealed class CommandEventUnsubscriber : IDisposable
        {
            private readonly ProcessMonitor _parent;
            private readonly IObserver<CommandEvent> _observer;

            public CommandEventUnsubscriber(ProcessMonitor parent, IObserver<CommandEvent> observer)
            {
                _parent = parent ?? throw new ArgumentNullException(nameof(parent));
                _observer = observer ?? throw new ArgumentNullException(nameof(observer));
            }

            public void Dispose()
            {
                if (_observer != null && _parent._observers.ContainsKey(_observer))
                {
                    _parent._observers.TryRemove(_observer, out _);
                }
            }
        }
        #endregion
    }

    public class ProcessMonitor<T> : ProcessMonitor
        where T : IProcess
    {
        public ProcessMonitor(T process, ILogger<ProcessMonitor<T>> logger)
            : base(process, logger)
        {
        }
    }
}
