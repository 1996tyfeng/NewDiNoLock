using System;

namespace NewDiNoLock.System
{
    [Serializable]
    public sealed class WindowSettings
    {
        public bool alwaysOnTop = true;
        public bool showAboveFullscreen;
        public bool autoHideInFullscreen = true;

        public static WindowSettings CreateDefault()
        {
            return new WindowSettings
            {
                alwaysOnTop = true,
                showAboveFullscreen = false,
                autoHideInFullscreen = true
            };
        }
    }
}
