namespace NewDiNoLock.Window
{
    public readonly struct DesktopMenuItem
    {
        public DesktopMenuItem(
            DesktopMenuCommand command,
            string label,
            bool isChecked = false,
            bool isEnabled = true,
            bool isSeparator = false)
        {
            Command = command;
            Label = label;
            IsChecked = isChecked;
            IsEnabled = isEnabled;
            IsSeparator = isSeparator;
        }

        public DesktopMenuCommand Command { get; }
        public string Label { get; }
        public bool IsChecked { get; }
        public bool IsEnabled { get; }
        public bool IsSeparator { get; }

        public static DesktopMenuItem Separator()
        {
            return new DesktopMenuItem(default, string.Empty, false, false, true);
        }
    }
}
