namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Event that is produced when a SimConnect data object is requested from SimConnect.
    /// </summary>
    public record SimObjectDataRequestedEvent : SimConnectEvent
    {
        public long RequestId
        {
            get;
            set;
        }

        public long DefinitionId
        {
            get;
            set;
        }

        public uint Radius
        {
            get;
            set;
        }

        public int SimObjectType
        {
            get;
            set;
        }
    }
}
