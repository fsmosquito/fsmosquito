namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Event that is produced when a SimConnect connection is closed. Usually if the game exits.
    /// </summary>
    public record SimConnectQuitEvent : SimConnectEvent
    {
    }
}
