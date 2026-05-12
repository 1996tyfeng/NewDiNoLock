using System;
using UnityEngine;

namespace NewDiNoLock.Window
{
    public interface IDesktopMenuService : IDisposable
    {
        event Action<DesktopMenuCommand> CommandSelected;

        bool IsSupported { get; }
        void Initialize(string tooltip, DesktopMenu menu);
        void SetMenu(DesktopMenu menu);
        bool ShowContextMenu(Vector2 screenPosition, DesktopMenu menu);
    }
}
