using System;

namespace NewDiNoLock.Window
{
    public readonly struct PlatformWindowHandle : IEquatable<PlatformWindowHandle>
    {
        public static readonly PlatformWindowHandle None = new PlatformWindowHandle(IntPtr.Zero);

        public PlatformWindowHandle(IntPtr value)
        {
            Value = value;
        }

        public IntPtr Value { get; }

        public bool IsValid => Value != IntPtr.Zero;

        public bool Equals(PlatformWindowHandle other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is PlatformWindowHandle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return IsValid ? $"0x{Value.ToInt64():X}" : "None";
        }
    }
}
