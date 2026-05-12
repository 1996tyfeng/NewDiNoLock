using UnityEngine;

namespace NewDiNoLock.Window
{
    public enum WindowZOrderMode
    {
        Normal,
        NormalTop,
        Topmost
    }

    public interface IWindowPlatformAdapter
    {
        bool TryGetMainWindowHandle(out PlatformWindowHandle handle);
        bool TryApplyTransparentBorderless(PlatformWindowHandle handle);
        bool TrySetZOrder(PlatformWindowHandle handle, WindowZOrderMode mode);
        bool TrySetClickThrough(PlatformWindowHandle handle, bool enabled);
        bool TrySetVisible(PlatformWindowHandle handle, bool visible);
        bool TryGetCursorClientPosition(PlatformWindowHandle handle, out Vector2 position);
    }
}
