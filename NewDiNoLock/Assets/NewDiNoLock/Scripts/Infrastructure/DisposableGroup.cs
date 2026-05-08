using System;
using System.Collections.Generic;

namespace NewDiNoLock.Infrastructure
{
    public sealed class DisposableGroup : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public int Count => _disposables.Count;

        public bool IsDisposed { get; private set; }

        public T Add<T>(T disposable) where T : IDisposable
        {
            if (disposable == null)
            {
                throw new ArgumentNullException(nameof(disposable));
            }

            if (IsDisposed)
            {
                disposable.Dispose();
                return disposable;
            }

            _disposables.Add(disposable);
            return disposable;
        }

        public bool Remove(IDisposable disposable)
        {
            if (disposable == null)
            {
                return false;
            }

            return _disposables.Remove(disposable);
        }

        public void Clear()
        {
            DisposeAll();
            _disposables.Clear();
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            DisposeAll();
            _disposables.Clear();
        }

        private void DisposeAll()
        {
            for (var index = _disposables.Count - 1; index >= 0; index--)
            {
                _disposables[index].Dispose();
            }
        }
    }
}
