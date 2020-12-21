namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Represents an abstract event that is produced by ISimConnect implementations.
    /// Use Pattern Matching to handle specific instances of this type.
    /// Can be either one of the following:
    /// <see cref="SimConnectOpenedEvent"/>,
    /// <see cref="SimConnectQuitEvent"/>,
    /// <see cref="SimObjectDataChangedEvent"/>,
    /// <see cref="SimObjectDataReceivedEvent"/>.
    /// <see cref="SimObjectDataRequestedEvent"/>.
    /// </summary>
    public abstract record SimConnectEvent
    {
    }
}
