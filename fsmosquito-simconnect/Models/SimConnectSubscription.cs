namespace FsMosquito.SimConnect
{
    using System;

    /// <summary>
    /// Represents a subscription to a SimConnect topic. Used internally by FsSimConnect to manage subscriptions to topics and monitor changes.
    /// </summary>
    internal sealed class SimConnectSubscription
    {
        /// <summary>
        /// Gets or sets the id of the subscription
        /// </summary>
        public long Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a SimConnect Topic
        /// </summary>
        public SimConnectTopic Topic
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates if the subscription data definition has been registered.
        /// </summary>
        public bool IsRegistered
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the id of a pending request.
        /// </summary>
        public long? PendingRequestId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the timestamp of when the most recent pending request started.
        /// </summary>
        public DateTime? PendingRequestStartTimeStamp
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last value of the subscription
        /// </summary>
        public object LastValue
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Topic.DatumName;
        }
    }
}
