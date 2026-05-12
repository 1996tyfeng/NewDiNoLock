using NewDiNoLock.System;
using UnityEngine;

namespace NewDiNoLock.Window
{
    public interface IDesktopWindowService
    {
        bool Apply(WindowSettings settings);
        bool SetClickThrough(bool enabled);
        bool SetVisible(bool visible);
        bool TryGetPointerClientPosition(out Vector2 position);
    }
}
