using NewDiNoLock.Infrastructure;
using NewDiNoLockSystem = NewDiNoLock.System;
using UnityEngine;

namespace NewDiNoLock.Bootstrap
{
    public sealed class AppBootstrapper : MonoBehaviour
    {
        private ServiceRegistry _services;
        private AppLifecycle _lifecycle;

        public ServiceRegistry Services => _services;
        public AppLifecycle Lifecycle => _lifecycle;

        private void Awake()
        {
            if (_lifecycle != null && _lifecycle.IsRunning)
            {
                return;
            }

            DontDestroyOnLoad(gameObject);
            Bootstrap();
        }

        private void OnApplicationQuit()
        {
            Shutdown();
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        public void Bootstrap()
        {
            _lifecycle = new AppLifecycle();
            _services = CreateDefaultServices(_lifecycle);
            _services.Resolve<NewDiNoLockSystem.ISettingsService>().Load();
            _lifecycle.MarkStarted();
        }

        public void Shutdown()
        {
            _lifecycle?.Shutdown();
            _services?.Dispose();
            _lifecycle = null;
            _services = null;
        }

        public static ServiceRegistry CreateDefaultServices(AppLifecycle lifecycle)
        {
            var services = new ServiceRegistry();

            var eventBus = new EventBus();
            var logger = new NewDiNoLock.Infrastructure.Logger();
            var timeProvider = new TimeProvider();
            var appPaths = new NewDiNoLockSystem.AppPaths();
            var localStorage = new NewDiNoLockSystem.LocalStorageService();
            var settingsService = new NewDiNoLockSystem.SettingsService(appPaths, localStorage, eventBus, logger);

            services.Register(lifecycle, false);
            services.Register<IEventBus>(eventBus);
            services.Register<NewDiNoLock.Infrastructure.ILogger>(logger);
            services.Register<ITimeProvider>(timeProvider);
            services.Register(appPaths);
            services.Register<NewDiNoLockSystem.ILocalStorageService>(localStorage);
            services.Register<NewDiNoLockSystem.ISettingsService>(settingsService);

            // Future modules should register their services here, then attach cleanup through AppLifecycle.
            // Window, Rendering, Core, and Features stay outside Bootstrap until their task cards implement them.

            return services;
        }
    }
}
