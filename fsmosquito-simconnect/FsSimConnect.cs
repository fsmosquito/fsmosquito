namespace FsMosquito.SimConnect
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.FlightSimulator.SimConnect;
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Timers;

    /// <summary>
    /// Represents a wrapper around SimConnect
    /// </summary>
    public sealed class FsSimConnect : IFsSimConnect
    {
        private const int PulseInterval = 500;
        private const int RequestWaitInterval = 1000 * 5;

        private static int s_subscriptionCount = 0;
        private static int s_currentRequestId = 0;

        private readonly ILogger<FsSimConnect> _logger;
        private readonly FsMosquitoOptions _options;
        private readonly Timer _pulseTimer = new Timer(PulseInterval);
        private readonly Timer _reconnectTimer = new Timer(15 * 1000);
        private readonly ConcurrentDictionary<string, SimConnectSubscription> _subscriptions = new ConcurrentDictionary<string, SimConnectSubscription>();
        private readonly ConcurrentDictionary<int, SimConnectSubscription> _pendingSubscriptions = new ConcurrentDictionary<int, SimConnectSubscription>();
        
        private IntPtr _lastHandle;
        private uint _messageId;
        private SimConnect _simConnect;

        public event EventHandler SimConnectOpened;
        public event EventHandler SimConnectClosed;
        public event EventHandler<(SimConnectTopic, uint, object)> TopicValueChanged;
        public event EventHandler SimConnectDataReceived;
        public event EventHandler SimConnectDataRequested;

        public FsSimConnect(IOptions<FsMosquitoOptions> options, ILogger<FsSimConnect> logger)
        {
            _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _pulseTimer.Elapsed += OnPulse_Elapsed;
            _reconnectTimer.Elapsed += (sender, args) =>
            {
                if (IsConnected == false)
                {
                    ConnectToSimConnect();
                }
            };

            if (_options.SimConnectMessageId.HasValue)
            {
                MessageId = (uint)options.Value.SimConnectMessageId.Value;
            }
            else
            {
                // Get a random WM_USER message number in the range of user messages
                var messageIdRng = new Random();
                var messageId = messageIdRng.Next(0x0400, 0x7FFF);
                MessageId = (uint)messageId;
            }
        }

        #region Properties
        public bool IsConnected
        {
            get
            {
                return _simConnect != null;
            }
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public bool IsOpen { get; private set; }

        public IntPtr LastHandle
        {
            get { return _lastHandle;  }
        }

        public uint MessageId
        {
            get
            {
                return _messageId;
            }
            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(FsSimConnect));
                }

                if (IsConnected)
                {
                    throw new InvalidOperationException("Unable to set MessageId while connected. Please disconnect first.");
                }

                _messageId = value;
            }
        }
        #endregion

        ///<inheritdoc />
        public void Connect(IntPtr handle)
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("FsMqtt is Disposed.");
            }

            if (IsConnected)
            {
                throw new InvalidOperationException("FsMqtt is already connected.");
            }

            _lastHandle = handle;
            ConnectToSimConnect();
        }

        /// <summary>
        /// Actually establishes the connection to SimConnect - try/catching the exception the contructor throws if FS isn't running.
        /// </summary>
        private void ConnectToSimConnect()
        {
            try
            {
                _logger.LogInformation($"FsMosquito attempting to connect to SimConnect using hWnd {_lastHandle} and WM_USER MessageId {MessageId}.");

                _simConnect = new SimConnect("FsMosquito", _lastHandle, MessageId, null, 0);

                _logger.LogInformation($"FsMosquito Connected to SimConnect using hWnd {_lastHandle} and WM_USER MessageId {MessageId}.");

                /// Listen to connect and quit msgs
                _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
                _simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

                // Listen to exceptions
                _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);

                // Listen to simobject data request
                _simConnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(SimConnect_OnRecvSimObjectDataByType);

                _reconnectTimer.Stop();

                // Subscribe to topics passed in via options
                if (_options != null && _options.TopicSubscriptions != null)
                {
                    foreach(var topic in _options.TopicSubscriptions)
                    {
                        Subscribe(topic);
                    }
                }
            }
            catch(Exception ex)
            {
                _simConnect = null;
                _logger.LogError($"FSMosquito was unable to connect to SimConnect: {ex.Message}", ex);
                _reconnectTimer.Start();
            }
        }

        ///<inheritdoc />
        public void Disconnect()
        {
            if (IsDisposed)
            {
                throw new InvalidOperationException("FsMqtt Is Disposed.");
            }

            if (!IsConnected)
            {
                throw new InvalidOperationException("FsMqtt is not connected.");
            }

            OnSimConnect_Closed();
        }

        ///<inheritdoc />
        public void Set(string datumName, uint? objectId, object value)
        {
            if (objectId.HasValue == false)
            {
                objectId = 0;
            }

            if (!_subscriptions.ContainsKey(datumName))
            {
                _logger.LogInformation($"Skipping setting value of {datumName} to {value} on object {objectId} as the topic has not been previously registered.");
            }

            var subscription = _subscriptions[datumName];

            var def = (Definition)Enum.ToObject(typeof(Definition), subscription.Id);
            _simConnect.SetDataOnSimObject(def, objectId.Value, SIMCONNECT_DATA_SET_FLAG.DEFAULT, value);

            // Cause the updated value to be re-transmitted next pulse.
            subscription.LastValue = null;
        }

        ///<inheritdoc />
        public void Subscribe(SimConnectTopic topic)
        {
            if (_subscriptions.ContainsKey(topic.DatumName))
            {
                _logger.LogInformation($"Skipping {topic.DatumName} as it has already been registered.");
                // This is verging on side-effect, but clear the last value so it will be re-transmitted next pulse.
                var subscription = _subscriptions[topic.DatumName];
                subscription.LastValue = null;
                return;
            }

            var newSubscription = topic.Units switch
            {
                Consts.SimConnectStringV => new SimConnectSubscription()
                {
                    Id = System.Threading.Interlocked.Increment(ref s_subscriptionCount),
                    Topic = topic,
                },
                _ => new SimConnectSubscription()
                {
                    Id = System.Threading.Interlocked.Increment(ref s_subscriptionCount),
                    Topic = topic,
                },
            };

            if (_subscriptions.TryAdd(topic.DatumName, newSubscription) == false)
            {
                return;
            }
            _logger.LogInformation($"Registered subscription for {topic.DatumName} with {topic.Units}.");
        }

        /// <summary>
        /// Registers a data definition/struct with SimConnect for the specified subscription.
        /// </summary>
        /// <param name="subscription"></param>
        private void RegisterSubscriptionDataDefinition(SimConnectSubscription subscription)
        {
            if (subscription.IsRegistered)
            {
                return;
            }

            if (IsDisposed || !IsConnected)
            {
                return;
            }

            var def = (Definition)Enum.ToObject(typeof(Definition), subscription.Id);
            var topic = subscription.Topic;
            try
            {
                switch (topic.Units)
                {
                    case Consts.SimConnectBool:
                        _simConnect.AddToDataDefinition(def, topic.DatumName, null, SIMCONNECT_DATATYPE.INT32, 0, SimConnect.SIMCONNECT_UNUSED);
                        _simConnect.RegisterDataDefineStruct<bool>(def);
                        break;
                    case Consts.SimConnectStringV:
                        _simConnect.AddToDataDefinition(def, topic.DatumName, null, SIMCONNECT_DATATYPE.STRINGV, 0, SimConnect.SIMCONNECT_UNUSED);
                        _simConnect.RegisterDataDefineStruct<StringStruct>(def);
                        break;
                    default:
                        /// Define a data structure
                        _simConnect.AddToDataDefinition(def, topic.DatumName, topic.Units, SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);

                        /// IMPORTANT: Register it with the simconnect managed wrapper marshaller
                        /// If you skip this step, you will only receive a uint in the .dwData field.
                        _simConnect.RegisterDataDefineStruct<double>(def);
                        break;
                }
                subscription.IsRegistered = true;
                _logger.LogInformation($"Registered Data Definition for {topic.DatumName} with {topic.Units}. ({subscription.Id})");
            }
            catch(Exception ex)
            {
                _logger.LogError($"An exception occurred attempting to register a data definition {ex.Message}", ex);
            }            
        }

        #region IObserver<SimConnectMessage>
        void IObserver<SimConnectMessage>.OnCompleted()
        {
            _logger.LogInformation($"Completed Observing SimConnect Messages.");

            OnSimConnect_Closed();
        }

        void IObserver<SimConnectMessage>.OnError(Exception error)
        {
            _logger.LogError($"An error occurred while observing SimConnectMessages: {error.Message}", error);

            OnSimConnect_Closed();

            _reconnectTimer.Start();
        }

        void IObserver<SimConnectMessage>.OnNext(SimConnectMessage value)
        {
            if (IsDisposed || !IsConnected || _simConnect == null)
            {
                return;
            }

            try
            {
                lock (_simConnect)
                {
                    _simConnect.ReceiveMessage();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while signaling to recieve a message: {ex.Message}", ex);

                OnSimConnect_Closed();

                _reconnectTimer.Start();
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Occurs when a connection is established to SimConnect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            IsOpen = true;
            _logger.LogInformation("SimConnect_OnRecvOpen");
            
            _pulseTimer.Start();
            OnSimConnect_Opened();
        }

        /// <summary>
        /// Occurs when the user closes the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            IsOpen = false;
            _logger.LogInformation("SimConnect_OnRecvQuit");
            OnSimConnect_Closed();
            
            _reconnectTimer.Start();            
        }

        /// <summary>
        /// Occurs when an exception is raised by SimConnect.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            var eException = (SIMCONNECT_EXCEPTION)data.dwException;

            _logger.LogInformation("SimConnect_OnRecvException: " + eException.ToString());
            OnSimConnect_Closed();

            _reconnectTimer.Start();
        }

        /// <summary>
        /// Occurs when SimConnect emits object data for a specific object type.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        private void SimConnect_OnRecvSimObjectDataByType(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            uint requestId = data.dwRequestID;
            uint objectId = data.dwObjectID;

            // ObjectID == 0 is the user object data. See SimConnect.SIMCONNECT_OBJECT_ID_USER;
            // But it seems to return it as 1... ???
            object currentValue;
            if (_pendingSubscriptions.TryRemove((int)requestId, out SimConnectSubscription subscription))
            {
                switch (subscription.Topic.Units)
                {
                    case Consts.SimConnectBool:
                        currentValue = (bool)data.dwData[0];
                        break;
                    case Consts.SimConnectStringV:
                        currentValue = ((StringStruct)data.dwData[0]).value;
                        break;
                    default:
                        currentValue = (double)data.dwData[0];
                        break;
                }

                subscription.PendingRequestId = null;
                subscription.PendingRequestStartTimeStamp = null;

                if (subscription.LastValue == null || subscription.LastValue.Equals(currentValue) == false)
                {
                    if (subscription.LastValue != currentValue)
                    {
                        subscription.LastValue = currentValue;
                        OnTopicValue_Changed(subscription.Topic, objectId, currentValue);
                    }
                }
            }

            OnSimConnectDataRecieved();
        }

        private void OnSimConnect_Opened()
        {
            if (SimConnectOpened != null)
            {
                SimConnectOpened.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnSimConnect_Closed()
        {
            _pulseTimer.Stop();

            if (_simConnect != null)
            {
                _simConnect.Dispose();
                _simConnect = null;
            }

            // Since SimConnect is now closed, mark all subscriptions as not registered, and non-pending
            // Also, clear out the last value so that subscribers recieve an updated value on re-acquisition of signal.
            foreach(var subscription in _subscriptions.Values)
            {
                subscription.IsRegistered = false;
                subscription.PendingRequestId = null;
                subscription.LastValue = null;
            }

            // Raise the SimClosed event
            if (SimConnectClosed != null)
            {
                SimConnectClosed.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnTopicValue_Changed(SimConnectTopic topic, uint objectId, object value)
        {
            if (TopicValueChanged != null)
            {
                TopicValueChanged.Invoke(this, (topic, objectId, value));
            }
        }

        private void OnPulse_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_simConnect == null)
            {
                return;
            }

            // Reset all subscriptions that have waited beyond the wait timeout.
            foreach(var subscription in _subscriptions.Values.Where(s => s.PendingRequestStartTimeStamp.HasValue && s.PendingRequestStartTimeStamp.Value.AddMilliseconds(RequestWaitInterval) < DateTime.Now))
            {
                subscription.PendingRequestId = null;
                subscription.PendingRequestStartTimeStamp = null;
            }

            // Request data for all subscriptions which currently aren't waiting for data.
            foreach(var subscription in _subscriptions.Values.Where(s => s.PendingRequestId.HasValue == false))
            {
                RegisterSubscriptionDataDefinition(subscription);

                var nextRequestId = GetNextRequestId();
                if (_pendingSubscriptions.TryAdd(nextRequestId, subscription) == false)
                {
                    continue;
                }

                subscription.PendingRequestId = nextRequestId;
                subscription.PendingRequestStartTimeStamp = DateTime.Now;
                var req = (Request)Enum.ToObject(typeof(Request), subscription.PendingRequestId);
                var def = (Definition)Enum.ToObject(typeof(Definition), subscription.Id);
                _simConnect.RequestDataOnSimObjectType(req, def, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);

                OnSimConnectDataRequested();
            }
        }

        private void OnSimConnectDataRecieved()
        {
            if (SimConnectDataReceived != null)
            {
                SimConnectDataReceived.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnSimConnectDataRequested()
        {
            if (SimConnectDataRequested != null)
            {
                SimConnectDataRequested.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        #region IDisposable
        private void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    if (null != _simConnect)
                    {
                        _pulseTimer.Stop();
                        _pulseTimer.Dispose();

                        _reconnectTimer.Stop();
                        _reconnectTimer.Dispose();

                        _simConnect.Dispose();
                        _simConnect = null;
                    }
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct StringStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            public string value;
        };
        #endregion

        #region Static Methods
        /// <summary>
        /// Gets the next request id (threadsafe, non-overflowing... although if we get to int.maxvalue, that's a lot of seconds)
        /// </summary>
        /// <returns></returns>
        private static int GetNextRequestId()
        {
            System.Threading.Interlocked.Increment(ref s_currentRequestId);
            return System.Threading.Interlocked.CompareExchange(ref s_currentRequestId, 0, int.MaxValue - 1);
        }
        #endregion
    }
}
