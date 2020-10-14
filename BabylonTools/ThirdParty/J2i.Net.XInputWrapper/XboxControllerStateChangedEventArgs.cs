using System;

namespace J2i.Net.XInputWrapper
{
    public class XboxControllerStateChangedEventArgs : EventArgs
    {
        public XInputState CurrentInputState { get; set; }
        public XInputState PreviousInputState { get; set; }
    }
}