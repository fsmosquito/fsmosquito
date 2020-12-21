namespace FsMosquito.SimConnect
{
    using System;

    /// <summary>
    /// Represents a SimConnect Windows Message Event Source - usually a Windows Form or lower level abstraction.
    /// </summary>
    public interface ISimConnectEventSource : IObservable<SimConnectWindowsMessageEvent>
    {
        /// <summary>
        /// Gets the HWnd of the Event Source
        /// </summary>
        IntPtr Handle
        {
            get;
        }
    }
}
