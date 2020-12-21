namespace FsMosquito.SimConnect
{
    using System;

    /// <summary>
    /// Represents an interface of a SimConnect wrapper implementation.
    /// </summary>
    public interface ISimConnect : IObserver<SimConnectWindowsMessageEvent>, IObservable<SimConnectEvent>, IDisposable
    {
        /// <summary>
        /// Gets or sets the message id that will be used for simconnect messages.
        /// </summary>
        uint MessageId { get; set; }

        /// <summary>
        /// Gets a value that indicates if the current instance has been able to create a SimConnect instance
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a value that indicates if the current instance has been disposed
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets a value that indicates if the SimConnect connection is open and active
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Start receiving messages from SimConnect
        /// </summary>
        /// <param name="handle"></param>
        void Connect(IntPtr handle);

        /// <summary>
        /// Stops receiving messages from SimConnect
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Sets a SimConnect Datum value on the specified object id with the indicated value
        /// </summary>
        /// <param name="datumName"></param>
        /// <param name="objectId"></param>
        /// <param name="value"></param>
        void Set(string datumName, uint? objectId, object value);

        /// <summary>
        /// Subscribes to a SimConnect Datum Topic
        /// </summary>
        /// <param name="topic"></param>
        void Subscribe(SimConnectTopic topic);
    }
}