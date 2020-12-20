namespace FsMosquito.SimConnect
{
    using System;

    public interface ISimConnectEventSource : IObservable<SimConnectMessage>
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
