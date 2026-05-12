using NewDiNoLock.System;

namespace NewDiNoLock.Window
{
    public sealed class AlwaysOnTopService
    {
        public WindowZOrderMode ResolveZOrder(WindowSettings settings)
        {
            settings ??= WindowSettings.CreateDefault();

            if (!settings.alwaysOnTop)
            {
                return WindowZOrderMode.Normal;
            }

            return WindowZOrderMode.Topmost;
        }

        public bool Apply(IWindowPlatformAdapter adapter, PlatformWindowHandle handle, WindowSettings settings)
        {
            return adapter.TrySetZOrder(handle, ResolveZOrder(settings));
        }
    }
}
