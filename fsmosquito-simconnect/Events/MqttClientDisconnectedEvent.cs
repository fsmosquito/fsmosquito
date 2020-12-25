namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Event that is produced when the MqttClient is disconnected
    /// </summary>
    public record MqttClientDisconnectedEvent : MqttClientEvent
    {
        public string Reason
        {
            get;
            set;
        }
    }
}
