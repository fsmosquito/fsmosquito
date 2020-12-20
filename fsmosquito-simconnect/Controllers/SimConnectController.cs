namespace FsMosquito.SimConnect.Controllers
{
    using FsMosquito.Attributes;
    using FsMosquito.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using MQTTnet;
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    [MqttController]
    public class SimConnectController
    {
        private readonly FsMosquitoOptions _options;
        private readonly ILogger<SimConnectController> _logger;
        private readonly IApplicationMessagePublisher _messagePublisher;
        private readonly IFsSimConnect _simConnect;

        public SimConnectController(IOptions<FsMosquitoOptions> options, IApplicationMessagePublisher messagePublisher, IFsSimConnect simConnect, ILogger<SimConnectController> logger)
        {
            _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
            _simConnect = simConnect ?? throw new ArgumentNullException(nameof(simConnect));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [EventPattern("fsm/atc/ident")]
        public async Task RequestControllerIdentification(MqttApplicationMessage message)
        {
            await _messagePublisher.PublishAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/ident")
                    .WithPayload(JsonSerializer.Serialize(HostInfo.GetHostInfo()))
                    .Build(),
                CancellationToken.None
                );

            _logger.LogInformation($"SimConnect Controller on {_options.HostName} Sent Host Info.");
        }

        [EventPattern("fsm/atc/{HostName}/status/pulse")]
        public async Task RequestPulseStatus(MqttApplicationMessage message)
        {
            await _messagePublisher.PublishAsync(
                 new MqttApplicationMessageBuilder()
                    .WithTopic($"fsm/atc/{_options.HostName}/status")
                    .WithPayload(_simConnect.IsConnected ? "Opened" : "Closed")
                    .Build(),
                 CancellationToken.None
            );

            _logger.LogInformation($"SimConnect Controller on {_options.HostName} sent SimConnect status ({_simConnect.IsConnected})");
        }

        [EventPattern("fsm/atc/{HostName}/obj/+/+/subscribe")]
        public void Subscribe(MqttApplicationMessage message)
        {
            // We're ignoring the object type for now...
            var objectType = message.GetTopicSegment(5);

            var topicsJson = Encoding.UTF8.GetString(message.Payload, 0, message.Payload.Length);
            var topics = JsonSerializer.Deserialize<SimConnectTopic[]>(topicsJson);
            if (_simConnect.IsConnected)
            {
                foreach (var topic in topics)
                {
                    _simConnect.Subscribe(topic);
                    _logger.LogInformation($"SimConnect Controller subscribed to {objectType} - {message.Topic}.");
                }
            }
        }

        [EventPattern("fsm/atc/{HostName}/obj/+/+/set")]
        public void SetSimVar(MqttApplicationMessage message)
        {
            var objectId = message.GetTopicSegment(4);
            var datumName = Regex.Replace(message.GetTopicSegment(5).ToUpperInvariant(), "_", " ");
            if (!uint.TryParse(objectId, out uint uintObjectId))
            {
                uintObjectId = 0;
            }

            var metricValueJson = Encoding.UTF8.GetString(message.Payload, 0, message.Payload.Length);
            var metricValue = JsonSerializer.Deserialize<SetSimConnectVarRequest>(metricValueJson);

            if (_simConnect.IsConnected)
            {
                _simConnect.Set(datumName, uintObjectId, metricValue.Value);
                _logger.LogInformation($"SimConnect Controller on {_options.HostName} set datum {datumName}.");
            }
        }
    }
}
