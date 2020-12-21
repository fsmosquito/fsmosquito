namespace FsMosquito.SimConnect
{
    using System;

    /// <summary>
    /// Represents a SimConnect Windows Message produced by SimConnect by a ISimConnectEventSource
    /// </summary>
    public record SimConnectWindowsMessageEvent : SimConnectEvent
    {
        public IntPtr HWnd { get; set; }
        public IntPtr LParam { get; set; }
        public int Msg { get; set; }
        public IntPtr WParam { get; set; }
    }
}
