namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Event that is produced when a previously subscribed SimObject data value has changed.
    /// </summary>
    public record SimObjectDataChangedEvent : SimConnectEvent
    {
        public SimConnectTopic Topic
        {
            get;
            set;
        }

        public uint ObjectId
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }
    }
}
