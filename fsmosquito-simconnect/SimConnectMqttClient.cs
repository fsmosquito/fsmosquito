namespace FsMosquito.SimConnect
{
    using FsMosquito.Extensions;
    using FsMosquito.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Disconnecting;
    using MQTTnet.Client.Options;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class SimConnectMqttClient : ISimConnectMqttClient
    {
        private static readonly Random s_random = new Random();

        private readonly FsMosquitoOptions _options;
        private readonly MqttApplicationMessageRouter _router;
        private readonly IMqttClientOptions _mqttClientOptions;
        private readonly ILogger<SimConnectMqttClient> _logger;

        public SimConnectMqttClient(IOptions<FsMosquitoOptions> options, MqttApplicationMessageRouter router, ILogger<SimConnectMqttClient> logger)
        {
            _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
            _router = router ?? throw new ArgumentNullException(nameof(router));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            MqttBrokerUrl = _options.MqttBrokerUrl;

            _mqttClientOptions = new MqttClientOptionsBuilder()
               .WithWebSocketServer(MqttBrokerUrl)
               .WithCredentials(_options.Username, _options.Password)
               .WithKeepAlivePeriod(TimeSpan.FromMilliseconds(_options.KeepAlivePeriodMs ?? 10000))
               .WithCommunicationTimeout(TimeSpan.FromMilliseconds(_options.CommunicationTimeoutMs ?? 15000))
               .WithWillDelayInterval(_options.WillDelayIntervalMs ?? 25000)
               .WithWillMessage(
                    new MqttApplicationMessageBuilder()
                        .WithPayloadFormatIndicator(MQTTnet.Protocol.MqttPayloadFormatIndicator.CharacterData)
                        .WithContentType("text/plain")
                        .WithTopic($"fsm/atc/{_options.HostName}/status")
                        .WithPayload("Disconnected")
                        .WithRetainFlag(true)
                        .Build()
               )
               .WithCleanSession()
               .Build();

            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            mqttClient.UseConnectedHandler(OnConnected);
            mqttClient.UseDisconnectedHandler(OnDisconnected);

            MqttClient = mqttClient;
        }

        public bool IsConnected
        {
            get
            {
                return MqttClient.IsConnected;
            }
        }

        public bool IsDisposed
        {
            get;
            private set;
        }

        public string MqttBrokerUrl
        {
            get;
        }

        public IMqttClient MqttClient
        {
            get;
        }

        public async Task Connect()
        {
            _logger.LogInformation($"Attempting to connect to {MqttBrokerUrl}...");
            await MqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
        }

        public async Task Disconnect()
        {
            await MqttClient.DisconnectAsync();
        }

        private async Task OnConnected(MqttClientConnectedEventArgs e)
        {
            _logger.LogInformation($"Connected to {MqttBrokerUrl}.");

            MqttClient.SubscribeControllers(_router);

            // Report that we've connected.
            await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/status")
                    .WithPayload("Connected")
                    .WithRetainFlag(true)
                    .Build()
                  );
        }

        private async Task OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            _logger.LogInformation($"Attempting to reconnect to {MqttBrokerUrl}.");
            await Task.Delay(TimeSpan.FromSeconds(s_random.Next(2, 12) * 5));

            try
            {
                await MqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Reconnection failed.");
                _logger.LogError($"Exception thrown on Reconnect: {ex.Message}", ex);
            }

            _logger.LogInformation($"Disconnected from {MqttBrokerUrl}.");
        }

        #region IDisposable Support
        private bool _isDisposed = false;

        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {

                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
