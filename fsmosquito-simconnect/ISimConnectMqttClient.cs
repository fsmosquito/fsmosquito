namespace FsMosquito.SimConnect
{
    using MQTTnet.Client;
    using System.Threading.Tasks;

    public interface ISimConnectMqttClient
    {
        /// <summary>
        /// Gets a value that indicates if the current instance is connected to MQTT
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Gets a value that indicates if the current instance has been disposed
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Gets the url of the Mqtt Broker that the implementation is using
        /// </summary>
        string MqttBrokerUrl { get; }

        /// <summary>
        /// Gets the underlying MqttClient implementation.
        /// </summary>
        IMqttClient MqttClient { get; }

        /// <summary>
        /// Start receiving messages from MQTT
        /// </summary>
        Task Connect();

        /// <summary>
        /// Stops receiving messages from MQTT
        /// </summary>
        Task Disconnect();
    }
}
