namespace FsMosquito.SimConnect
{
    using System;

    /// <summary>
    /// Adapts the functionality of SimConnect to another messaging mechanism.
    /// </summary>
    public interface ISimConnectAdapter : IObserver<SimConnectEvent>
    {
    }
}