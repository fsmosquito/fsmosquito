namespace FsMosquito.Mqtt
{
    using MQTTnet;
    using MQTTnet.Server;

    /// <summary>
    /// Represents a MQTT Service (Used for DI)
    /// </summary>
    public interface IMqttService
    {
        IApplicationMessagePublisher Publisher { get; }

        void ConfigureMqttServer(IMqttServer server);
    }
}