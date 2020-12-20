namespace FsMosquito.SimConnect
{
    using System;

    /// <summary>
    /// Represents a SimConnect Message
    /// </summary>
    public record SimConnectMessage
    {
        public IntPtr HWnd { get; set; }
        public IntPtr LParam { get; set; }
        public int Msg { get; set; }
        public IntPtr WParam { get; set; }
    }
}
