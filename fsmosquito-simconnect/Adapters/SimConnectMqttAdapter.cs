namespace FsMosquito.SimConnect
{
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MQTTnet;
    using System;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Adapts the functionality of SimConnect to MQTT Based Messaging.
    /// </summary>
    public class SimConnectMqttAdapter : ISimConnectAdapter
    {
        private readonly FsMosquitoOptions _options;
        private readonly IApplicationMessagePublisher _messagePublisher;
        private readonly ILogger<SimConnectMqttAdapter> _logger;

        public SimConnectMqttAdapter(IOptions<FsMosquitoOptions> options, IApplicationMessagePublisher messagePublisher, ILogger<SimConnectMqttAdapter> logger)
        {
            _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SimConnectOpened()
        {
            await _messagePublisher.PublishAsync(
                 new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/status")
                    .WithPayload("Opened")
                    .Build(),
                 CancellationToken.None
            );

            _logger.LogInformation($"Published SimConnect on {_options.HostName} Opened Status");
        }

        public async Task SimConnectClosed()
        {
            await _messagePublisher.PublishAsync(
                 new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/status")
                    .WithPayload("Closed")
                    .Build(),
                 CancellationToken.None
            );

            _logger.LogInformation($"Published SimConnect on {_options.HostName} Closed Status");
        }

        public async Task TopicValueChanged((SimConnectTopic topic, uint objectId, object value) topicValue)
        {
            var snakeCasedTopicName = Regex.Replace(topicValue.topic.DatumName.ToLower(), "\\s", "_");
            await _messagePublisher.PublishAsync(
                 new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/obj/{topicValue.objectId}/{snakeCasedTopicName}")
                    .WithPayload(JsonSerializer.Serialize(new SimConnectValue() { Units = topicValue.topic.Units, Value = topicValue.value } ))
                    .Build(),
                 CancellationToken.None
            );
        }
    }
}
