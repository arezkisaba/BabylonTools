using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace J2i.Net.XInputWrapper
{
    [StructLayout(LayoutKind.Explicit)]
    public struct XInputState
    {
        [FieldOffset(0)] public int PacketNumber;

        [FieldOffset(4)] public XInputGamepad Gamepad;

        public void Copy(XInputState source)
        {
            PacketNumber = source.PacketNumber;
            Gamepad.Copy(source.Gamepad);
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || (!(obj is XInputState)))
                return false;
            var source = (XInputState) obj;

            return ((PacketNumber == source.PacketNumber)
                    && (Gamepad.Equals(source.Gamepad)));
        }

        public override int GetHashCode()
        {
            var hashCode = -1459879740;
            hashCode = hashCode * -1521134295 + PacketNumber.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<XInputGamepad>.Default.GetHashCode(Gamepad);
            return hashCode;
        }
    }
}