using NewDiNoLock.System;
using UnityEngine;
using ILogger = NewDiNoLock.Infrastructure.ILogger;

namespace NewDiNoLock.Window
{
    public sealed class NoOpDesktopWindowService : IDesktopWindowService
    {
        private readonly ILogger _logger;

        public NoOpDesktopWindowService(ILogger logger)
        {
            _logger = logger;
        }

        public bool Apply(WindowSettings settings)
        {
            _logger?.Debug("Desktop window settings are a no-op outside Windows player builds.");
            return true;
        }

        public bool SetClickThrough(bool enabled)
        {
            return true;
        }

        public bool SetVisible(bool visible)
        {
            return true;
        }

        public bool TryGetPointerClientPosition(out Vector2 position)
        {
            position = Input.mousePosition;
            return true;
        }
    }
}
