namespace FsMosquito.Mqtt
{
    using MQTTnet;
    using MQTTnet.Server;

    public interface IMqttService
    {
        IApplicationMessagePublisher Publisher { get; }

        void ConfigureMqttServer(IMqttServer server);
    }
}