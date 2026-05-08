using System;
using System.Collections.Generic;

namespace NewDiNoLock.Bootstrap
{
    public sealed class ServiceRegistry : IDisposable
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
        private readonly List<IDisposable> _ownedDisposables = new List<IDisposable>();

        public void Register<TService>(TService service, bool disposeWithRegistry = true)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            _services[typeof(TService)] = service;

            if (disposeWithRegistry && service is IDisposable disposable && !_ownedDisposables.Contains(disposable))
            {
                _ownedDisposables.Add(disposable);
            }
        }

        public bool TryResolve<TService>(out TService service)
        {
            if (_services.TryGetValue(typeof(TService), out var value))
            {
                service = (TService)value;
                return true;
            }

            service = default;
            return false;
        }

        public TService Resolve<TService>()
        {
            if (TryResolve(out TService service))
            {
                return service;
            }

            throw new InvalidOperationException($"Service is not registered: {typeof(TService).FullName}");
        }

        public void Dispose()
        {
            for (var index = _ownedDisposables.Count - 1; index >= 0; index--)
            {
                _ownedDisposables[index].Dispose();
            }

            _ownedDisposables.Clear();
            _services.Clear();
        }
    }
}
