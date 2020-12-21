namespace FsMosquito.SimConnect
{
    using FsMosquito.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MQTTnet;
    using System;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading;

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

        public void OnCompleted()
        {
            _messagePublisher.PublishAsync(
                 new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/status")
                    .WithPayload("Completed")
                    .Build(),
                 CancellationToken.None
            ).Forget();

            _logger.LogInformation($"Published SimConnect on {_options.HostName} Opened Status");
        }

        public void OnError(Exception error)
        {
            _messagePublisher.PublishAsync(
                 new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/status")
                    .WithPayload("Error")
                    .Build(),
                 CancellationToken.None
            ).Forget();
        }

        public void OnNext(SimConnectEvent simEvent)
        {
            switch (simEvent)
            {
                case SimConnectOpenedEvent:
                    _messagePublisher.PublishAsync(
                         new MqttApplicationMessageBuilder()
                            .WithTopic($"fsm/atc/{_options.HostName}/status")
                            .WithPayload("Opened")
                            .Build(),
                         CancellationToken.None
                    ).Forget();

                    _logger.LogInformation($"Published SimConnect on {_options.HostName} Opened Status");
                    break;
                case SimConnectQuitEvent:
                    _messagePublisher.PublishAsync(
                         new MqttApplicationMessageBuilder()
                            .WithTopic($"fsm/atc/{_options.HostName}/status")
                            .WithPayload("Closed")
                            .Build(),
                         CancellationToken.None
                    ).Forget();

                    _logger.LogInformation($"Published SimConnect on {_options.HostName} Closed Status");
                    break;
                case SimObjectDataChangedEvent changed:
                    var dataChangedTopicFragmentName = Regex.Replace(changed.Topic.DatumName.ToLower(), "\\s", "_");
                    if (!string.IsNullOrWhiteSpace(changed.Topic.TopicLevelName))
                    {
                        dataChangedTopicFragmentName = changed.Topic.TopicLevelName.Trim();
                    }

                    _messagePublisher.PublishAsync(
                         new MqttApplicationMessageBuilder()
                            .WithTopic($"fsm/atc/{_options.HostName}/obj/{changed.ObjectId}/{dataChangedTopicFragmentName}")
                            .WithPayload(JsonSerializer.Serialize(new SimObjectMeasurement() { Units = changed.Topic.Units, Value = changed.Value }))
                            .Build(),
                         CancellationToken.None
                    ).Forget();
                    break;
                default:
                    // Ignore SimObjectDataRecieved, SimObjectDataRequested for now.
                    break;
            }
        }
    }
}
