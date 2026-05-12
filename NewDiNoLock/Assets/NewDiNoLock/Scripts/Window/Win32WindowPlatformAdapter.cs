using UnityEngine;
using ILogger = NewDiNoLock.Infrastructure.ILogger;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
#endif

namespace NewDiNoLock.Window
{
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    internal sealed class Win32WindowPlatformAdapter : IWindowPlatformAdapter
    {
        private const int GwlStyle = -16;
        private const int GwlExStyle = -20;

        private const long WsCaption = 0x00C00000L;
        private const long WsThickFrame = 0x00040000L;
        private const long WsMinimizeBox = 0x00020000L;
        private const long WsMaximizeBox = 0x00010000L;
        private const long WsSysMenu = 0x00080000L;
        private const long WsExLayered = 0x00080000L;
        private const long WsExTransparent = 0x00000020L;
        private const long WsExToolWindow = 0x00000080L;
        private const long WsExAppWindow = 0x00040000L;

        private const uint LwaAlpha = 0x00000002;
        private const byte WindowAlpha = 255;

        private const uint SwpNoSize = 0x0001;
        private const uint SwpNoMove = 0x0002;
        private const uint SwpNoActivate = 0x0010;
        private const uint SwpFrameChanged = 0x0020;
        private const uint SwpShowWindow = 0x0040;
        private const int SwHide = 0;
        private const int SwShow = 5;

        private static readonly IntPtr HwndTop = IntPtr.Zero;
        private static readonly IntPtr HwndTopmost = new IntPtr(-1);
        private static readonly IntPtr HwndNoTopmost = new IntPtr(-2);

        private readonly ILogger _logger;

        public Win32WindowPlatformAdapter(ILogger logger)
        {
            _logger = logger;
        }

        public bool TryGetMainWindowHandle(out PlatformWindowHandle handle)
        {
            var hwnd = GetActiveWindow();
            handle = new PlatformWindowHandle(hwnd);

            if (!handle.IsValid)
            {
                _logger?.Warning("GetActiveWindow returned no Unity window handle.");
                return false;
            }

            return true;
        }

        public bool TryApplyTransparentBorderless(PlatformWindowHandle handle)
        {
            if (!handle.IsValid)
            {
                return false;
            }

            if (!TryGetWindowLongPtr(handle.Value, GwlStyle, out var style))
            {
                return false;
            }

            var newStyle = new IntPtr(style.ToInt64() & ~(WsCaption | WsThickFrame | WsMinimizeBox | WsMaximizeBox | WsSysMenu));
            if (!TrySetWindowLongPtr(handle.Value, GwlStyle, newStyle))
            {
                return false;
            }

            if (!TryGetWindowLongPtr(handle.Value, GwlExStyle, out var exStyle))
            {
                return false;
            }

            var newExStyle = new IntPtr((exStyle.ToInt64() | WsExLayered | WsExToolWindow) & ~WsExAppWindow);
            if (!TrySetWindowLongPtr(handle.Value, GwlExStyle, newExStyle))
            {
                return false;
            }

            if (!SetLayeredWindowAttributes(handle.Value, 0, WindowAlpha, LwaAlpha))
            {
                LogLastError("SetLayeredWindowAttributes");
                return false;
            }

            var margins = Margins.FullWindow;
            var hresult = DwmExtendFrameIntoClientArea(handle.Value, ref margins);
            if (hresult != 0)
            {
                _logger?.Warning($"DwmExtendFrameIntoClientArea failed. HRESULT: 0x{hresult:X8}");
                return false;
            }

            return TrySetWindowPos(handle.Value, HwndTop, SwpFrameChanged);
        }

        public bool TrySetZOrder(PlatformWindowHandle handle, WindowZOrderMode mode)
        {
            if (!handle.IsValid)
            {
                return false;
            }

            switch (mode)
            {
                case WindowZOrderMode.Normal:
                    return TrySetWindowPos(handle.Value, HwndNoTopmost);
                case WindowZOrderMode.NormalTop:
                    return TrySetWindowPos(handle.Value, HwndNoTopmost) && TrySetWindowPos(handle.Value, HwndTop);
                case WindowZOrderMode.Topmost:
                    return TrySetWindowPos(handle.Value, HwndTopmost);
                default:
                    return TrySetWindowPos(handle.Value, HwndNoTopmost);
            }
        }

        public bool TrySetClickThrough(PlatformWindowHandle handle, bool enabled)
        {
            if (!handle.IsValid)
            {
                return false;
            }

            if (!TryGetWindowLongPtr(handle.Value, GwlExStyle, out var exStyle))
            {
                return false;
            }

            var exStyleValue = exStyle.ToInt64();
            var newExStyle = enabled
                ? new IntPtr(exStyleValue | WsExTransparent | WsExLayered)
                : new IntPtr((exStyleValue & ~WsExTransparent) | WsExLayered);

            if (!TrySetWindowLongPtr(handle.Value, GwlExStyle, newExStyle))
            {
                return false;
            }

            return TrySetWindowPos(handle.Value, HwndTop, SwpFrameChanged);
        }

        public bool TrySetVisible(PlatformWindowHandle handle, bool visible)
        {
            if (!handle.IsValid)
            {
                return false;
            }

            _ = ShowWindow(handle.Value, visible ? SwShow : SwHide);
            return true;
        }

        public bool TryGetCursorClientPosition(PlatformWindowHandle handle, out Vector2 position)
        {
            position = default;
            if (!handle.IsValid)
            {
                return false;
            }

            if (!GetCursorPos(out var point))
            {
                LogLastError("GetCursorPos");
                return false;
            }

            if (!ScreenToClient(handle.Value, ref point))
            {
                LogLastError("ScreenToClient");
                return false;
            }

            position = new Vector2(point.X, Screen.height - point.Y);
            return true;
        }

        private bool TrySetWindowPos(IntPtr hwnd, IntPtr insertAfter, uint extraFlags = 0)
        {
            var flags = SwpNoMove | SwpNoSize | SwpNoActivate | SwpShowWindow | extraFlags;
            if (SetWindowPos(hwnd, insertAfter, 0, 0, 0, 0, flags))
            {
                return true;
            }

            LogLastError("SetWindowPos");
            return false;
        }

        private bool TryGetWindowLongPtr(IntPtr hwnd, int index, out IntPtr value)
        {
            SetLastError(0);
            value = IntPtr.Size == 8
                ? GetWindowLongPtr64(hwnd, index)
                : new IntPtr(GetWindowLong32(hwnd, index));

            var lastError = Marshal.GetLastWin32Error();
            if (value == IntPtr.Zero && lastError != 0)
            {
                _logger?.Warning($"GetWindowLongPtr failed. Index: {index}. LastError: {lastError}");
                return false;
            }

            return true;
        }

        private bool TrySetWindowLongPtr(IntPtr hwnd, int index, IntPtr value)
        {
            SetLastError(0);
            var previousValue = IntPtr.Size == 8
                ? SetWindowLongPtr64(hwnd, index, value)
                : new IntPtr(SetWindowLong32(hwnd, index, value.ToInt32()));

            var lastError = Marshal.GetLastWin32Error();
            if (previousValue == IntPtr.Zero && lastError != 0)
            {
                _logger?.Warning($"SetWindowLongPtr failed. Index: {index}. LastError: {lastError}");
                return false;
            }

            return true;
        }

        private void LogLastError(string apiName)
        {
            _logger?.Warning($"{apiName} failed. LastError: {Marshal.GetLastWin32Error()}");
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        private static extern int GetWindowLong32(IntPtr hwnd, int index);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hwnd, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern int SetWindowLong32(IntPtr hwnd, int index, int value);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hwnd, int index, IntPtr value);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint colorKey, byte alpha, uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hwnd, int command);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ScreenToClient(IntPtr hwnd, ref Point point);

        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins margins);

        [DllImport("kernel32.dll")]
        private static extern void SetLastError(uint errorCode);

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Margins
        {
            public int LeftWidth;
            public int RightWidth;
            public int TopHeight;
            public int BottomHeight;

            public static Margins FullWindow => new Margins
            {
                LeftWidth = -1,
                RightWidth = -1,
                TopHeight = -1,
                BottomHeight = -1
            };
        }
    }
#endif
}
