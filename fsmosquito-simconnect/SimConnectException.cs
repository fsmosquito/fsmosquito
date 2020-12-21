namespace FsMosquito.SimConnect
{
    using System;

    public sealed class SimConnectException : Exception
    {
        public SimConnectException()
            : base()
        {
        }

        public SimConnectException(string message)
            : base(message)
        { 
        }

        public SimConnectException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public uint ExceptionId
        {
            get;
            set;
        }

        public uint SendId
        {
            get;
            set;
        }

        public uint Index
        {
            get;
            set;
        }
    }
}
