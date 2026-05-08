namespace NewDiNoLock.Infrastructure
{
    public enum RuntimePlatformKind
    {
        Unknown,
        Windows,
        MacOS,
        Linux,
        Editor
    }

    public static class Platform
    {
        public static RuntimePlatformKind Current
        {
            get
            {
#if UNITY_EDITOR
                return RuntimePlatformKind.Editor;
#elif UNITY_STANDALONE_WIN
                return RuntimePlatformKind.Windows;
#elif UNITY_STANDALONE_OSX
                return RuntimePlatformKind.MacOS;
#elif UNITY_STANDALONE_LINUX
                return RuntimePlatformKind.Linux;
#else
                return RuntimePlatformKind.Unknown;
#endif
            }
        }

        public static bool IsEditor => Current == RuntimePlatformKind.Editor;
        public static bool IsWindows => Current == RuntimePlatformKind.Windows;
        public static bool IsMacOS => Current == RuntimePlatformKind.MacOS;
        public static bool IsLinux => Current == RuntimePlatformKind.Linux;
    }
}
