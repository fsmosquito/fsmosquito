namespace FsMosquito.Mqtt
{
    using Microsoft.Extensions.Logging;
    using MQTTnet;
    using MQTTnet.Server;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a MQTTNet MQTT service
    /// </summary>
    public class MqttService :
        IMqttServerClientConnectedHandler,
        IMqttServerClientDisconnectedHandler,
        IMqttServerClientSubscribedTopicHandler,
        IMqttService
    {
        private readonly ILogger<MqttService> _logger;
        private IMqttServer _mqtt;

        public MqttService(ILogger<MqttService> logger)
        {
            _logger = logger;
        }

        public IApplicationMessagePublisher Publisher
        {
            get { return _mqtt; }
        }

        public void ConfigureMqttServer(IMqttServer mqtt)
        {
            _mqtt = mqtt;
            mqtt.ClientConnectedHandler = this;
            mqtt.ClientDisconnectedHandler = this;
            mqtt.ClientSubscribedTopicHandler = this;
        }

        public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
        {
            _logger.LogInformation("Client connected.");
            return Task.CompletedTask;
        }

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            _logger.LogInformation("Client disconnected.");
            return Task.CompletedTask;
        }

        public Task HandleClientSubscribedTopicAsync(MqttServerClientSubscribedTopicEventArgs eventArgs)
        {
            _logger.LogInformation($"Topic Subscribed: {eventArgs.TopicFilter}");
            return Task.CompletedTask;
        }

        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            _logger.LogInformation("app message publish");
            return Task.CompletedTask;
        }
    }
}
