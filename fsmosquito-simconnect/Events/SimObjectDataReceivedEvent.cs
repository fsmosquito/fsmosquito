namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Event that is produced when a SimConnect data object is received from SimConnect
    /// </summary>
    public record SimObjectDataReceivedEvent : SimConnectEvent
    {
        /// <summary>
        /// Gets or sets the request id
        /// </summary>
        public uint RequestId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the object id 
        /// </summary>
        public uint ObjectId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of SimObject data that was recieved.
        /// If this Request/Object was not made as part of FsSimConnect, this value will be null.
        /// </summary>
        public object Value
        {
            get;
            set;
        }
    }
}
