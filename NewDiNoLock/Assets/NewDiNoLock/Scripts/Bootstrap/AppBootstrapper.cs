using NewDiNoLock.Core;
using NewDiNoLock.Infrastructure;
using NewDiNoLock.UI;
using NewDiNoLock.Window;
using NewDiNoLockSystem = NewDiNoLock.System;
using UnityEngine;

namespace NewDiNoLock.Bootstrap
{
    public sealed class AppBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private GameObject _defaultPetPrefab;

        [SerializeField]
        private bool _spawnDefaultPetIfMissing = true;

        [SerializeField]
        private bool _createCameraIfMissing = true;

        [SerializeField]
        private RightClickMenuPopup _rightClickMenuPrefab;

        private const string RightClickMenuPrefabPath = "Assets/NewDiNoLock/Prefabs/UI/RightClickMenu/RightClickMenuPopup.prefab";

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
            EnsureSceneObjects();
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

            var eventBus = _services.Resolve<IEventBus>();
            var settingsService = _services.Resolve<NewDiNoLockSystem.ISettingsService>();
            var windowService = _services.Resolve<IDesktopWindowService>();
            var menuService = _services.Resolve<IDesktopMenuService>();
            var settings = settingsService.Load();
            var menuController = EnsureMenuController(settingsService, windowService, menuService);

            windowService.Apply(settings.window);
            ConfigureClickThroughControllers(windowService);
            ConfigureContextMenus(menuController);
            ApplyPetSettings(settings.pet);
            var settingsSubscription = eventBus.Subscribe<NewDiNoLockSystem.SettingsChanged>(eventData =>
            {
                windowService.Apply(eventData.Settings.window);
                ConfigureClickThroughControllers(windowService);
                ConfigureContextMenus(menuController);
                ApplyPetSettings(eventData.Settings.pet);
                menuController.RefreshMenu();
            });
            _lifecycle.RegisterDisposable(settingsSubscription);
            _lifecycle.RegisterDisposable(menuController);

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
            var desktopWindowService = DesktopWindowService.CreateDefault(logger);
            var desktopMenuService = DesktopMenuService.CreateDefault(logger);

            services.Register(lifecycle, false);
            services.Register<IEventBus>(eventBus);
            services.Register<NewDiNoLock.Infrastructure.ILogger>(logger);
            services.Register<ITimeProvider>(timeProvider);
            services.Register(appPaths);
            services.Register<NewDiNoLockSystem.ILocalStorageService>(localStorage);
            services.Register<NewDiNoLockSystem.ISettingsService>(settingsService);
            services.Register<IDesktopWindowService>(desktopWindowService);
            services.Register<IDesktopMenuService>(desktopMenuService);

            // Future modules should register their services here, then attach cleanup through AppLifecycle.
            // Rendering and Features stay outside Bootstrap until their task cards implement them.

            return services;
        }

        private static void ApplyPetSettings(NewDiNoLockSystem.PetSettings petSettings)
        {
            var autoWalkEnabled = petSettings == null || petSettings.autoWalkEnabled;
            var movementControllers = Object.FindObjectsOfType<PetMovementController>();
            for (var index = 0; index < movementControllers.Length; index++)
            {
                movementControllers[index].SetAutoWalkEnabled(autoWalkEnabled);
            }
        }

        private static void ConfigureClickThroughControllers(IDesktopWindowService windowService)
        {
            var clickThroughControllers = Object.FindObjectsOfType<PetWindowClickThroughController>();
            for (var index = 0; index < clickThroughControllers.Length; index++)
            {
                clickThroughControllers[index].Configure(windowService);
            }
        }

        private SettingsMenuController EnsureMenuController(
            NewDiNoLockSystem.ISettingsService settingsService,
            IDesktopWindowService windowService,
            IDesktopMenuService menuService)
        {
            var menuController = GetComponent<SettingsMenuController>();
            if (menuController == null)
            {
                menuController = gameObject.AddComponent<SettingsMenuController>();
            }

            menuController.Configure(settingsService, windowService, menuService, ResolveRightClickMenuPrefab());
            return menuController;
        }

        private RightClickMenuPopup ResolveRightClickMenuPrefab()
        {
            if (_rightClickMenuPrefab != null)
            {
                return _rightClickMenuPrefab;
            }

#if UNITY_EDITOR
            _rightClickMenuPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<RightClickMenuPopup>(RightClickMenuPrefabPath);
#endif
            return _rightClickMenuPrefab;
        }

        private static void ConfigureContextMenus(SettingsMenuController menuController)
        {
            var petControllers = Object.FindObjectsOfType<PetBehaviorController>(true);
            for (var index = 0; index < petControllers.Length; index++)
            {
                var petObject = petControllers[index].gameObject;
                var contextMenu = petObject.GetComponent<ContextMenuView>();
                if (contextMenu == null)
                {
                    contextMenu = petObject.AddComponent<ContextMenuView>();
                }

                contextMenu.Configure(menuController);
            }
        }

        private void EnsureSceneObjects()
        {
            if (_createCameraIfMissing && Object.FindObjectOfType<Camera>() == null)
            {
                var cameraObject = new GameObject("Main Camera");
                var camera = cameraObject.AddComponent<Camera>();
                camera.tag = "MainCamera";
                camera.orthographic = true;
                camera.orthographicSize = 3f;
                camera.transform.position = new Vector3(0f, 0f, -10f);
            }

            var sceneCamera = Camera.main != null ? Camera.main : Object.FindObjectOfType<Camera>();
            if (sceneCamera != null)
            {
                ConfigureTransparentCamera(sceneCamera);
            }

            if (!_spawnDefaultPetIfMissing || _defaultPetPrefab == null || Object.FindObjectOfType<PetBehaviorController>() != null)
            {
                return;
            }

            var pet = Instantiate(_defaultPetPrefab);
            pet.name = _defaultPetPrefab.name;
            pet.transform.position = Vector3.zero;
        }

        private static void ConfigureTransparentCamera(Camera camera)
        {
            camera.clearFlags = CameraClearFlags.SolidColor;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
#else
            camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
#endif
        }
    }
}
