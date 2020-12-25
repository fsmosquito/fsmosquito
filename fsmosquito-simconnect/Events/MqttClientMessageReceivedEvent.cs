namespace FsMosquito.SimConnect
{
    /// <summary>
    /// Event that is produced when the MqttClient recieves a message.
    /// </summary>
    public record MqttClientMessageReceivedEvent : MqttClientEvent
    {
        public string Topic
        {
            get;
            set;
        }
    }
}
