using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17
{
    public interface ICommand
    {
        void Execute(CancellationToken token);
    }

    public class SoftStopCommand : ICommand
    {
        internal void ExecuteInternal()
        {
            if (Thread.CurrentThread != ServerThread.CurrentThread)
                throw new InvalidOperationException("SoftStop must be executed by the target thread.");
        }

        public void Execute(CancellationToken token) => ExecuteInternal();
    }

    public class HardStopCommand : ICommand
    {
        internal void ExecuteInternal()
        {
            if (Thread.CurrentThread != ServerThread.CurrentThread)
                throw new InvalidOperationException("HardStop must be executed by the target thread.");
        }

        public void Execute(CancellationToken token) => ExecuteInternal();
    }

    public static class ExceptionHandler
    {
        public static void HandleException(ICommand command, Exception exception)
        {
            Console.WriteLine($"Exception in {command?.GetType().Name ?? "unknown command"}: {exception.Message}");
        }
    }

    public class ServerThread
    {
        public static Thread? CurrentThread { get; private set; }

        private readonly BlockingCollection<ICommand> _queue = new();
        private Thread _thread;
        private bool _isSoftStopRequested;
        private readonly CancellationTokenSource _cts = new();
        private readonly object _lock = new();

        public ServerThread()
        {
            _thread = new Thread(() => WorkerLoop(_cts.Token)) { IsBackground = true };
            CurrentThread = _thread;
            _thread.Start();
        }

        public void EnqueueCommand(ICommand command)
        {
            lock (_lock)
            {
                if (_queue.IsAddingCompleted)
                    throw new InvalidOperationException("Cannot enqueue command: thread is stopped.");
            }
            _queue.Add(command);
        }

        private void WorkerLoop(CancellationToken token)
        {
            try
            {
                foreach (var command in _queue.GetConsumingEnumerable(token))
                {
                    if (_isSoftStopRequested)
                        break;

                    if (command is HardStopCommand hardStop)
                    {
                        hardStop.ExecuteInternal();
                        break;
                    }

                    if (command is SoftStopCommand softStop)
                    {
                        softStop.ExecuteInternal();
                        _isSoftStopRequested = true;
                        break;
                    }

                    try
                    {
                        command.Execute(token);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.HandleException(command, ex);
                    }
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                // игнорируем — нормальное завершение
            }
            finally
            {
                _queue.CompleteAdding();
                _cts.Dispose();
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_isSoftStopRequested || _queue.IsAddingCompleted)
                    throw new InvalidOperationException("Thread is already stopping or stopped.");
            }
            EnqueueCommand(new SoftStopCommand());
        }

        public void ForceStop()
        {
            lock (_lock)
            {
                if (_queue.IsAddingCompleted)
                    return; // уже остановлен

                _cts.Cancel();
                _queue.CompleteAdding(); // останавливаем очередь
            }
        }

        public void Join(int millisecondsTimeout = Timeout.Infinite) => _thread.Join(millisecondsTimeout);
        public void Join() => _thread.Join();
        public bool IsAlive => _thread.IsAlive;
    }
}
