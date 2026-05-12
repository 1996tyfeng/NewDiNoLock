using System;
using NewDiNoLock.System;
using UnityEngine;
using ILogger = NewDiNoLock.Infrastructure.ILogger;

namespace NewDiNoLock.Window
{
    public sealed class DesktopWindowService : IDesktopWindowService
    {
        private readonly IWindowPlatformAdapter _adapter;
        private readonly AlwaysOnTopService _alwaysOnTopService;
        private readonly ILogger _logger;
        private PlatformWindowHandle _windowHandle;
        private WindowZOrderMode _lastZOrderMode;
        private bool _hasAppliedZOrder;

        public DesktopWindowService(IWindowPlatformAdapter adapter, ILogger logger)
            : this(adapter, new AlwaysOnTopService(), logger)
        {
        }

        public DesktopWindowService(IWindowPlatformAdapter adapter, AlwaysOnTopService alwaysOnTopService, ILogger logger)
        {
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
            _alwaysOnTopService = alwaysOnTopService ?? throw new ArgumentNullException(nameof(alwaysOnTopService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static IDesktopWindowService CreateDefault(ILogger logger)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return new DesktopWindowService(new Win32WindowPlatformAdapter(logger), logger);
#else
            return new NoOpDesktopWindowService(logger);
#endif
        }

        public bool Apply(WindowSettings settings)
        {
            settings ??= WindowSettings.CreateDefault();

            if (!EnsureWindowHandle())
            {
                _logger.Warning("Desktop window settings were skipped because the application window handle is not available yet.");
                return false;
            }

            var styleApplied = _adapter.TryApplyTransparentBorderless(_windowHandle);
            if (!styleApplied)
            {
                _logger.Warning("Failed to apply transparent borderless window style. Continuing startup.");
            }

            var zOrderMode = _alwaysOnTopService.ResolveZOrder(settings);
            var zOrderApplied = _adapter.TrySetZOrder(_windowHandle, zOrderMode);
            if (!zOrderApplied)
            {
                _logger.Warning("Failed to apply desktop window z-order settings. Continuing startup.");
            }
            else
            {
                _lastZOrderMode = zOrderMode;
                _hasAppliedZOrder = true;
            }

            return styleApplied && zOrderApplied;
        }

        public bool SetClickThrough(bool enabled)
        {
            if (!EnsureWindowHandle())
            {
                _logger.Warning("Desktop window click-through was skipped because the application window handle is not available yet.");
                return false;
            }

            var changed = _adapter.TrySetClickThrough(_windowHandle, enabled);
            if (!changed)
            {
                _logger.Warning($"Failed to set desktop window click-through to {enabled}. Continuing.");
            }

            if (_hasAppliedZOrder && !_adapter.TrySetZOrder(_windowHandle, _lastZOrderMode))
            {
                _logger.Warning("Failed to restore desktop window z-order after changing click-through. Continuing.");
            }

            return changed;
        }

        public bool SetVisible(bool visible)
        {
            if (!EnsureWindowHandle())
            {
                _logger.Warning("Desktop window visibility change was skipped because the application window handle is not available yet.");
                return false;
            }

            var changed = _adapter.TrySetVisible(_windowHandle, visible);
            if (!changed)
            {
                _logger.Warning($"Failed to set desktop window visibility to {visible}. Continuing.");
                return false;
            }

            if (visible && _hasAppliedZOrder && !_adapter.TrySetZOrder(_windowHandle, _lastZOrderMode))
            {
                _logger.Warning("Failed to restore desktop window z-order after showing the window. Continuing.");
            }

            return true;
        }

        public bool TryGetPointerClientPosition(out Vector2 position)
        {
            position = default;
            return EnsureWindowHandle() && _adapter.TryGetCursorClientPosition(_windowHandle, out position);
        }

        private bool EnsureWindowHandle()
        {
            if (_windowHandle.IsValid)
            {
                return true;
            }

            if (!_adapter.TryGetMainWindowHandle(out _windowHandle))
            {
                _windowHandle = PlatformWindowHandle.None;
                return false;
            }

            return _windowHandle.IsValid;
        }
    }
}
