using System;
using NewDiNoLock.System;
using NewDiNoLock.Window;
using UnityEngine;

namespace NewDiNoLock.UI
{
    [DisallowMultipleComponent]
    public sealed class RightClickMenuPopup : MonoBehaviour
    {
        [SerializeField]
        private Transform _menuRoot;

        private SettingsMenuController _controller;
        private RectTransform _rectTransform;
        private RectTransform _hitRectTransform;
        private RightClickMenuButtonElement[] _buttonElements = Array.Empty<RightClickMenuButtonElement>();
        private RightClickMenuToggleElement[] _toggleElements = Array.Empty<RightClickMenuToggleElement>();
        private bool _isRefreshing;
        private int _openedFrame;

        private void Awake()
        {
            EnsureReferences();
            RegisterCallbacks();
        }

        private void Update()
        {
            if (Time.frameCount == _openedFrame || _controller == null)
            {
                return;
            }

            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1))
            {
                return;
            }

            if (!IsPointerInside(Input.mousePosition))
            {
                _controller.HideContextMenu();
            }
        }

        private void OnDestroy()
        {
            UnregisterCallbacks();
        }

        public void Configure(SettingsMenuController controller)
        {
            _controller = controller;
            Refresh();
        }

        public void Show(Vector2 anchoredPosition)
        {
            EnsureReferences();
            Refresh();
            gameObject.SetActive(true);
            _rectTransform.anchoredPosition = anchoredPosition;
            _openedFrame = Time.frameCount;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Refresh()
        {
            if (_controller == null)
            {
                return;
            }

            var settings = _controller.CurrentSettings;
            settings.Normalize();
            EnsureReferences();

            _isRefreshing = true;
            for (var index = 0; index < _toggleElements.Length; index++)
            {
                var toggle = _toggleElements[index];
                if (toggle == null)
                {
                    continue;
                }

                toggle.SetDefaultValue(GetToggleValue(toggle.Command, settings));
            }

            _isRefreshing = false;
        }

        private void OnToggleChanged(DesktopMenuCommand command, bool enabled)
        {
            if (_isRefreshing)
            {
                return;
            }

            switch (command)
            {
                case DesktopMenuCommand.ToggleAutoWalk:
                    _controller?.SetAutoWalk(enabled);
                    break;
                case DesktopMenuCommand.ToggleAlwaysOnTop:
                    _controller?.SetAlwaysOnTop(enabled);
                    break;
                case DesktopMenuCommand.ToggleShowAboveFullscreen:
                    _controller?.SetShowAboveFullscreen(enabled);
                    break;
                default:
                    _controller?.ExecuteCommand(command);
                    break;
            }
        }

        private void ExecuteAndClose(DesktopMenuCommand command)
        {
            _controller?.ExecuteCommand(command);
            _controller?.HideContextMenu();
        }

        private bool IsPointerInside(Vector2 screenPosition)
        {
            EnsureReferences();
            return _hitRectTransform != null && RectTransformUtility.RectangleContainsScreenPoint(_hitRectTransform, screenPosition);
        }

        private void RegisterCallbacks()
        {
            EnsureReferences();
            for (var index = 0; index < _buttonElements.Length; index++)
            {
                var element = _buttonElements[index];
                if (element == null)
                {
                    continue;
                }

                element.Clicked -= ExecuteAndClose;
                element.Clicked += ExecuteAndClose;
            }

            for (var index = 0; index < _toggleElements.Length; index++)
            {
                var element = _toggleElements[index];
                if (element == null)
                {
                    continue;
                }

                element.ValueChanged -= OnToggleChanged;
                element.ValueChanged += OnToggleChanged;
            }
        }

        private void UnregisterCallbacks()
        {
            if (_buttonElements != null)
            {
                for (var index = 0; index < _buttonElements.Length; index++)
                {
                    var element = _buttonElements[index];
                    if (element != null)
                    {
                        element.Clicked -= ExecuteAndClose;
                    }
                }
            }

            if (_toggleElements != null)
            {
                for (var index = 0; index < _toggleElements.Length; index++)
                {
                    var element = _toggleElements[index];
                    if (element != null)
                    {
                        element.ValueChanged -= OnToggleChanged;
                    }
                }
            }
        }

        private void EnsureReferences()
        {
            if (_rectTransform == null)
            {
                _rectTransform = transform as RectTransform;
            }

            if (_hitRectTransform == null)
            {
                _hitRectTransform = _menuRoot as RectTransform;
            }

            if (_menuRoot != null && (_buttonElements == null || _buttonElements.Length == 0))
            {
                _buttonElements = _menuRoot.GetComponentsInChildren<RightClickMenuButtonElement>(true);
            }

            if (_menuRoot != null && (_toggleElements == null || _toggleElements.Length == 0))
            {
                _toggleElements = _menuRoot.GetComponentsInChildren<RightClickMenuToggleElement>(true);
            }
        }

        private static bool GetToggleValue(DesktopMenuCommand command, AppSettings settings)
        {
            switch (command)
            {
                case DesktopMenuCommand.ToggleAutoWalk:
                    return settings.pet.autoWalkEnabled;
                case DesktopMenuCommand.ToggleAlwaysOnTop:
                    return settings.window.alwaysOnTop;
                case DesktopMenuCommand.ToggleShowAboveFullscreen:
                    return settings.window.showAboveFullscreen;
                default:
                    return false;
            }
        }

        private static Transform FindChildRecursive(Transform root, string childName)
        {
            for (var index = 0; index < root.childCount; index++)
            {
                var child = root.GetChild(index);
                if (string.Equals(child.name, childName, StringComparison.Ordinal))
                {
                    return child;
                }

                var result = FindChildRecursive(child, childName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
