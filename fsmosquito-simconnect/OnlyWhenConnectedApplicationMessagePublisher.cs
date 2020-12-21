namespace FsMosquito.SimConnect
{
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Publishing;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a Application Message Publisher which only sends messages if the underlying IMqttClient is connected.
    /// Yes, this is a little silly, but otherwise the OOTB IMqttClient throws exceptions, maybe rightly so.
    /// </summary>
    public sealed class OnlyWhenConnectedApplicationMessagePublisher : IApplicationMessagePublisher
    {
        private readonly IMqttClient _client;

        public OnlyWhenConnectedApplicationMessagePublisher(IMqttClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage applicationMessage, CancellationToken cancellationToken)
        {
            if (_client.IsConnected == false)
            {
                return Task.FromResult<MqttClientPublishResult>(null);
            }

            return _client.PublishAsync(applicationMessage, cancellationToken);
        }
    }
}
