using System;
using System.Collections.Generic;

namespace NewDiNoLock.Window
{
    public sealed class DesktopMenu
    {
        private readonly IReadOnlyList<DesktopMenuItem> _items;

        public DesktopMenu(IReadOnlyList<DesktopMenuItem> items)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
        }

        public IReadOnlyList<DesktopMenuItem> Items => _items;

        public static DesktopMenu Empty { get; } = new DesktopMenu(Array.Empty<DesktopMenuItem>());

        public static DesktopMenu From(params DesktopMenuItem[] items)
        {
            return new DesktopMenu(items ?? Array.Empty<DesktopMenuItem>());
        }
    }
}
