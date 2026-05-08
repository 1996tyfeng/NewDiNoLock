using System;
using System.Collections.Generic;

namespace NewDiNoLock.Bootstrap
{
    public sealed class AppLifecycle : IDisposable
    {
        private readonly List<Action> _shutdownSteps = new List<Action>();

        public bool IsRunning { get; private set; }
        public bool IsShutdown { get; private set; }

        public void MarkStarted()
        {
            if (IsShutdown)
            {
                throw new InvalidOperationException("Cannot start after shutdown.");
            }

            IsRunning = true;
        }

        public void RegisterShutdownAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (IsShutdown)
            {
                action();
                return;
            }

            _shutdownSteps.Add(action);
        }

        public T RegisterDisposable<T>(T disposable) where T : IDisposable
        {
            if (disposable == null)
            {
                throw new ArgumentNullException(nameof(disposable));
            }

            if (IsShutdown)
            {
                disposable.Dispose();
                return disposable;
            }

            _shutdownSteps.Add(disposable.Dispose);
            return disposable;
        }

        public void Shutdown()
        {
            if (IsShutdown)
            {
                return;
            }

            IsRunning = false;
            IsShutdown = true;

            for (var index = _shutdownSteps.Count - 1; index >= 0; index--)
            {
                _shutdownSteps[index]();
            }

            _shutdownSteps.Clear();
        }

        public void Dispose()
        {
            Shutdown();
        }
    }
}
