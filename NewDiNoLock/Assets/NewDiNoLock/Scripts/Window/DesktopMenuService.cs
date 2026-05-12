using UnityEngine;
using ILogger = NewDiNoLock.Infrastructure.ILogger;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System;
using System.Runtime.InteropServices;
using System.Text;
#endif

namespace NewDiNoLock.Window
{
    public static class DesktopMenuService
    {
        public static IDesktopMenuService CreateDefault(ILogger logger)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return new Win32DesktopMenuService(logger);
#else
            return new NoOpDesktopMenuService();
#endif
        }
    }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
    internal sealed class Win32DesktopMenuService : IDesktopMenuService
    {
        private const uint TrayIconId = 1;
        private const uint WmAppTrayIcon = 0x8001;
        private const int WmRButtonUp = 0x0205;
        private const int WmLButtonDblClk = 0x0203;
        private const int WmNull = 0x0000;
        private const int GwlWndProc = -4;

        private const uint NimAdd = 0x00000000;
        private const uint NimModify = 0x00000001;
        private const uint NimDelete = 0x00000002;
        private const uint NifMessage = 0x00000001;
        private const uint NifIcon = 0x00000002;
        private const uint NifTip = 0x00000004;

        private const uint MfString = 0x00000000;
        private const uint MfGray = 0x00000001;
        private const uint MfChecked = 0x00000008;
        private const uint MfSeparator = 0x00000800;

        private const uint TpmRightButton = 0x00000002;
        private const uint TpmReturnCommand = 0x00000100;
        private const uint TpmNonotify = 0x00000080;

        private const int FirstCommandId = 1000;

        private readonly ILogger _logger;
        private readonly WndProcDelegate _wndProcDelegate;
        private DesktopMenu _menu = DesktopMenu.Empty;
        private IntPtr _hwnd;
        private IntPtr _previousWndProc;
        private bool _trayAdded;
        private bool _disposed;
        private IntPtr _trayIcon;

        public Win32DesktopMenuService(ILogger logger)
        {
            _logger = logger;
            _wndProcDelegate = WindowProc;
        }

        public event Action<DesktopMenuCommand> CommandSelected;

        public bool IsSupported => true;

        public void Initialize(string tooltip, DesktopMenu menu)
        {
            if (_disposed)
            {
                return;
            }

            _menu = menu ?? DesktopMenu.Empty;
            if (!EnsureWindowHandle() || !EnsureSubclassed())
            {
                return;
            }

            var data = CreateNotifyIconData(tooltip);
            var action = _trayAdded ? NimModify : NimAdd;
            if (!Shell_NotifyIcon(action, ref data))
            {
                LogLastError("Shell_NotifyIcon");
                return;
            }

            _trayAdded = true;
        }

        public void SetMenu(DesktopMenu menu)
        {
            _menu = menu ?? DesktopMenu.Empty;
        }

        public bool ShowContextMenu(Vector2 screenPosition, DesktopMenu menu)
        {
            _menu = menu ?? _menu ?? DesktopMenu.Empty;
            if (!EnsureWindowHandle())
            {
                return false;
            }

            var point = new Point
            {
                X = Mathf.RoundToInt(screenPosition.x),
                Y = Mathf.RoundToInt(Screen.height - screenPosition.y)
            };
            if (!ClientToScreen(_hwnd, ref point))
            {
                LogLastError("ClientToScreen");
                return false;
            }

            return ShowContextMenuAtScreenPoint(point, _menu);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_trayAdded)
            {
                var data = CreateNotifyIconData(string.Empty);
                Shell_NotifyIcon(NimDelete, ref data);
                _trayAdded = false;
            }

            if (_trayIcon != IntPtr.Zero)
            {
                DestroyIcon(_trayIcon);
                _trayIcon = IntPtr.Zero;
            }

            if (_hwnd != IntPtr.Zero && _previousWndProc != IntPtr.Zero)
            {
                SetWindowLongPtr(_hwnd, GwlWndProc, _previousWndProc);
                _previousWndProc = IntPtr.Zero;
            }

            CommandSelected = null;
        }

        private bool EnsureWindowHandle()
        {
            if (_hwnd != IntPtr.Zero)
            {
                return true;
            }

            _hwnd = GetActiveWindow();
            if (_hwnd == IntPtr.Zero)
            {
                _logger?.Warning("Tray menu could not find the Unity window handle.");
                return false;
            }

            return true;
        }

        private bool EnsureSubclassed()
        {
            if (_previousWndProc != IntPtr.Zero)
            {
                return true;
            }

            _previousWndProc = SetWindowLongPtr(_hwnd, GwlWndProc, Marshal.GetFunctionPointerForDelegate(_wndProcDelegate));
            if (_previousWndProc == IntPtr.Zero)
            {
                LogLastError("SetWindowLongPtr(GWL_WNDPROC)");
                return false;
            }

            return true;
        }

        private NotifyIconData CreateNotifyIconData(string tooltip)
        {
            return new NotifyIconData
            {
                cbSize = (uint)Marshal.SizeOf<NotifyIconData>(),
                hWnd = _hwnd,
                uID = TrayIconId,
                uFlags = NifMessage | NifIcon | NifTip,
                uCallbackMessage = WmAppTrayIcon,
                hIcon = ResolveTrayIcon(),
                szTip = string.IsNullOrWhiteSpace(tooltip) ? "NewDiNoLock" : tooltip
            };
        }

        private IntPtr ResolveTrayIcon()
        {
            if (_trayIcon != IntPtr.Zero)
            {
                return _trayIcon;
            }

            var modulePathBuilder = new StringBuilder(260);
            var length = GetModuleFileName(IntPtr.Zero, modulePathBuilder, modulePathBuilder.Capacity);
            if (length == 0)
            {
                LogLastError("GetModuleFileName");
                return IntPtr.Zero;
            }

            var modulePath = modulePathBuilder.ToString();
            _trayIcon = ExtractIcon(IntPtr.Zero, modulePath, 0);
            if (_trayIcon == IntPtr.Zero)
            {
                LogLastError("ExtractIcon");
            }

            return _trayIcon;
        }

        private IntPtr WindowProc(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == WmAppTrayIcon)
            {
                var mouseMessage = lParam.ToInt32();
                if (mouseMessage == WmRButtonUp)
                {
                    if (GetCursorPos(out var point))
                    {
                        ShowContextMenuAtScreenPoint(point, _menu);
                    }

                    return IntPtr.Zero;
                }

                if (mouseMessage == WmLButtonDblClk)
                {
                    CommandSelected?.Invoke(DesktopMenuCommand.ToggleVisibility);
                    return IntPtr.Zero;
                }
            }

            return CallWindowProc(_previousWndProc, hwnd, message, wParam, lParam);
        }

        private bool ShowContextMenuAtScreenPoint(Point point, DesktopMenu menu)
        {
            var popupMenu = CreatePopupMenu();
            if (popupMenu == IntPtr.Zero)
            {
                LogLastError("CreatePopupMenu");
                return false;
            }

            try
            {
                var commandIndex = 0;
                for (var index = 0; index < menu.Items.Count; index++)
                {
                    var item = menu.Items[index];
                    if (item.IsSeparator)
                    {
                        AppendMenu(popupMenu, MfSeparator, 0, string.Empty);
                        continue;
                    }

                    var flags = MfString;
                    if (item.IsChecked)
                    {
                        flags |= MfChecked;
                    }

                    if (!item.IsEnabled)
                    {
                        flags |= MfGray;
                    }

                    AppendMenu(popupMenu, flags, FirstCommandId + commandIndex, item.Label);
                    commandIndex++;
                }

                SetForegroundWindow(_hwnd);
                var selectedCommandId = TrackPopupMenuEx(
                    popupMenu,
                    TpmRightButton | TpmReturnCommand | TpmNonotify,
                    point.X,
                    point.Y,
                    _hwnd,
                    IntPtr.Zero);
                PostMessage(_hwnd, WmNull, IntPtr.Zero, IntPtr.Zero);

                if (selectedCommandId == 0)
                {
                    return true;
                }

                InvokeCommandById(menu, selectedCommandId);
                return true;
            }
            finally
            {
                DestroyMenu(popupMenu);
            }
        }

        private void InvokeCommandById(DesktopMenu menu, int selectedCommandId)
        {
            var targetIndex = selectedCommandId - FirstCommandId;
            var commandIndex = 0;
            for (var index = 0; index < menu.Items.Count; index++)
            {
                var item = menu.Items[index];
                if (item.IsSeparator)
                {
                    continue;
                }

                if (commandIndex == targetIndex)
                {
                    if (item.IsEnabled)
                    {
                        CommandSelected?.Invoke(item.Command);
                    }

                    return;
                }

                commandIndex++;
            }
        }

        private void LogLastError(string apiName)
        {
            _logger?.Warning($"{apiName} failed. LastError: {Marshal.GetLastWin32Error()}");
        }

        private delegate IntPtr WndProcDelegate(IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct NotifyIconData
        {
            public uint cbSize;
            public IntPtr hWnd;
            public uint uID;
            public uint uFlags;
            public uint uCallbackMessage;
            public IntPtr hIcon;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szTip;

            public uint dwState;
            public uint dwStateMask;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szInfo;

            public uint uTimeoutOrVersion;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string szInfoTitle;

            public uint dwInfoFlags;
            public Guid guidItem;
            public IntPtr hBalloonIcon;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hwnd, int index, IntPtr newLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallWindowProc(IntPtr previousWndProc, IntPtr hwnd, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool Shell_NotifyIcon(uint message, ref NotifyIconData data);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern uint GetModuleFileName(IntPtr module, StringBuilder filename, int size);

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr ExtractIcon(IntPtr instance, string exeFileName, uint iconIndex);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyIcon(IntPtr icon);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreatePopupMenu();

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AppendMenu(IntPtr menu, uint flags, int itemId, string itemText);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int TrackPopupMenuEx(IntPtr menu, uint flags, int x, int y, IntPtr hwnd, IntPtr parameters);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyMenu(IntPtr menu);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out Point point);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ClientToScreen(IntPtr hwnd, ref Point point);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam);
    }
#endif
}
