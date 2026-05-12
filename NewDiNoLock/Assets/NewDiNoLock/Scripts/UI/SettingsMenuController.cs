using System;
using NewDiNoLock.Core;
using NewDiNoLock.System;
using NewDiNoLock.Window;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NewDiNoLock.UI
{
    [DisallowMultipleComponent]
    public sealed class SettingsMenuController : MonoBehaviour, IDisposable
    {
        private ISettingsService _settingsService;
        private IDesktopWindowService _windowService;
        private IDesktopMenuService _menuService;
        private RightClickMenuPopup _rightClickMenuPrefab;
        private RightClickMenuPopup _rightClickMenu;
        private Canvas _menuCanvas;
        private bool _isPetVisible = true;
        private bool _isContextMenuVisible;

        public bool IsPetVisible => _isPetVisible;
        public AppSettings CurrentSettings => _settingsService?.Current ?? AppSettings.CreateDefault();

        public void Configure(
            ISettingsService settingsService,
            IDesktopWindowService windowService,
            IDesktopMenuService menuService,
            RightClickMenuPopup rightClickMenuPrefab)
        {
            Dispose();

            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _windowService = windowService ?? throw new ArgumentNullException(nameof(windowService));
            _menuService = menuService ?? throw new ArgumentNullException(nameof(menuService));
            _rightClickMenuPrefab = rightClickMenuPrefab;
            _menuService.CommandSelected += HandleCommandSelected;
            _menuService.Initialize("NewDiNoLock", BuildMenu());
            EnsureRightClickMenu();
        }

        public void ShowContextMenu(Vector2 screenPosition)
        {
            if (_rightClickMenuPrefab == null)
            {
                return;
            }

            EnsureRightClickMenu();
            if (_rightClickMenu == null || _menuCanvas == null)
            {
                return;
            }

            var canvasTransform = (RectTransform)_menuCanvas.transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, screenPosition, null, out var localPosition);
            _rightClickMenu.Show(localPosition);
            _isContextMenuVisible = true;
            SetAutoWalkPausedForMenu(true);
            _windowService?.SetClickThrough(false);
        }

        public void HideContextMenu()
        {
            if (!_isContextMenuVisible && (_rightClickMenu == null || !_rightClickMenu.gameObject.activeSelf))
            {
                return;
            }

            _rightClickMenu?.Hide();
            _isContextMenuVisible = false;
            SetAutoWalkPausedForMenu(false);
        }

        public void RefreshMenu()
        {
            _menuService?.SetMenu(BuildMenu());
            _rightClickMenu?.Refresh();
        }

        public void SetPetVisible(bool visible)
        {
            if (_isPetVisible == visible)
            {
                RefreshMenu();
                return;
            }

            var behaviors = FindObjectsOfType<PetBehaviorController>(true);
            if (visible)
            {
                _windowService?.SetVisible(true);
                for (var index = 0; index < behaviors.Length; index++)
                {
                    behaviors[index].Show("menu show");
                }
            }
            else
            {
                for (var index = 0; index < behaviors.Length; index++)
                {
                    behaviors[index].RequestHide("menu hide");
                }

                _windowService?.SetVisible(false);
            }

            _isPetVisible = visible;
            RefreshMenu();
        }

        public void Dispose()
        {
            if (_menuService != null)
            {
                _menuService.CommandSelected -= HandleCommandSelected;
            }

            _menuService = null;
            _settingsService = null;
            _windowService = null;
            _rightClickMenuPrefab = null;
            _isContextMenuVisible = false;
            SetAutoWalkPausedForMenu(false);
        }

        public void ExecuteCommand(DesktopMenuCommand command)
        {
            HandleCommandSelected(command);
        }

        public void SetAutoWalk(bool enabled)
        {
            UpdateSettings(settings => settings.pet.autoWalkEnabled = enabled);
            RefreshMenu();
        }

        public void SetAlwaysOnTop(bool enabled)
        {
            UpdateSettings(settings => settings.window.alwaysOnTop = enabled);
            RefreshMenu();
        }

        public void SetShowAboveFullscreen(bool enabled)
        {
            UpdateSettings(settings => settings.window.showAboveFullscreen = enabled);
            RefreshMenu();
        }

        private DesktopMenu BuildMenu()
        {
            var settings = _settingsService?.Current ?? AppSettings.CreateDefault();
            settings.Normalize();

            return DesktopMenu.From(
                new DesktopMenuItem(
                    DesktopMenuCommand.ToggleVisibility,
                    _isPetVisible ? "Hide Pet" : "Show Pet"),
                DesktopMenuItem.Separator(),
                new DesktopMenuItem(
                    DesktopMenuCommand.ToggleAutoWalk,
                    "Auto Walk",
                    settings.pet.autoWalkEnabled),
                new DesktopMenuItem(
                    DesktopMenuCommand.ToggleAlwaysOnTop,
                    "Always On Top",
                    settings.window.alwaysOnTop),
                new DesktopMenuItem(
                    DesktopMenuCommand.ToggleShowAboveFullscreen,
                    "Show Above Fullscreen",
                    settings.window.showAboveFullscreen),
                DesktopMenuItem.Separator(),
                new DesktopMenuItem(DesktopMenuCommand.OpenSettings, "Settings"),
                new DesktopMenuItem(DesktopMenuCommand.Exit, "Exit"));
        }

        private void HandleCommandSelected(DesktopMenuCommand command)
        {
            switch (command)
            {
                case DesktopMenuCommand.ToggleVisibility:
                    SetPetVisible(!_isPetVisible);
                    break;
                case DesktopMenuCommand.ToggleAutoWalk:
                    UpdateSettings(settings => settings.pet.autoWalkEnabled = !settings.pet.autoWalkEnabled);
                    break;
                case DesktopMenuCommand.ToggleAlwaysOnTop:
                    UpdateSettings(settings => settings.window.alwaysOnTop = !settings.window.alwaysOnTop);
                    break;
                case DesktopMenuCommand.ToggleShowAboveFullscreen:
                    UpdateSettings(settings => settings.window.showAboveFullscreen = !settings.window.showAboveFullscreen);
                    break;
                case DesktopMenuCommand.OpenSettings:
                    Debug.Log("Settings menu selected. Settings panel is not implemented yet.");
                    break;
                case DesktopMenuCommand.Exit:
                    QuitApplication();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, "Unsupported menu command.");
            }

            RefreshMenu();
        }

        private void UpdateSettings(Action<AppSettings> update)
        {
            if (_settingsService == null)
            {
                return;
            }

            _settingsService.Update(update);
        }

        private void EnsureRightClickMenu()
        {
            EnsureEventSystem();
            if (_menuCanvas == null)
            {
                var canvasObject = new GameObject("Right Click Menu Canvas", typeof(RectTransform));
                canvasObject.transform.SetParent(transform, false);
                _menuCanvas = canvasObject.AddComponent<Canvas>();
                _menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _menuCanvas.sortingOrder = short.MaxValue;
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            if (_rightClickMenu == null)
            {
                _rightClickMenu = Instantiate(_rightClickMenuPrefab, _menuCanvas.transform);
                _rightClickMenu.Configure(this);
                _rightClickMenu.Hide();
            }
        }

        private static void SetAutoWalkPausedForMenu(bool paused)
        {
            var movementControllers = FindObjectsOfType<PetMovementController>(true);
            for (var index = 0; index < movementControllers.Length; index++)
            {
                movementControllers[index].SetAutoWalkPaused(paused);
            }
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private static void QuitApplication()
        {
            Application.Quit();
        }
    }
}
