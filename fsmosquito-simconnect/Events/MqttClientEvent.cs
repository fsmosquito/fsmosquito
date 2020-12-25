namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Represents an abstract event that is produced by ISimConnectMqttClient implementations
    /// </summary>
    /// Use Pattern Matching to handle specific instances of this type.
    /// Can be either one of the following:
    /// <see cref="MqttClientConnectedEvent"/>,
    /// <see cref="MqttClientDisconnectedEvent"/>,
    /// <see cref="MqttClientMessageReceivedEvent"/>,
    /// </summary>
    public abstract record MqttClientEvent
    {
    }
}
