using System;
using UnityEngine;

namespace NewDiNoLock.Window
{
    public sealed class NoOpDesktopMenuService : IDesktopMenuService
    {
        public event Action<DesktopMenuCommand> CommandSelected
        {
            add { }
            remove { }
        }

        public bool IsSupported => false;

        public void Initialize(string tooltip, DesktopMenu menu)
        {
        }

        public void SetMenu(DesktopMenu menu)
        {
        }

        public bool ShowContextMenu(Vector2 screenPosition, DesktopMenu menu)
        {
            return false;
        }

        public void Dispose()
        {
        }
    }
}
