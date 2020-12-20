namespace FsMosquito
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record FsMosquitoOptions
    {
        public FsMosquitoOptions()
        {
            ApiKey = "1234";
            MqttBrokerUrl = "ws://localhost:5272/mqtt";
            HostName = Environment.MachineName;
            Username = "";
            Password = "";
            KeepAlivePeriodMs = 10000; // Default: 10s
            ReconnectDelayMs = 15000; // Default: 15s
            WillDelayIntervalMs = 25000; // Default: 25s
        }

        [JsonPropertyName("apiKey")]
        public string ApiKey
        {
            get;
            set;
        }

        [JsonPropertyName("mqttBrokerUrl")]
        public string MqttBrokerUrl
        {
            get;
            set;
        }

        [JsonPropertyName("simConnectMessageId")]
        public int? SimConnectMessageId
        {
            get;
            set;
        }

        [JsonPropertyName("hostName")]
        public string HostName
        {
            get;
            set;
        }

        [JsonPropertyName("username")]
        public string Username
        {
            get;
            set;
        }

        [JsonPropertyName("password")]
        public string Password
        {
            get;
            set;
        }

        [JsonPropertyName("keepAlivePeriodMs")]
        public int? KeepAlivePeriodMs
        {
            get;
            set;
        }

        [JsonPropertyName("communicationTimeoutMs")]
        public int? CommunicationTimeoutMs
        {
            get;
            set;
        }

        [JsonPropertyName("reconnectDelayMs")]
        public uint? ReconnectDelayMs
        {
            get;
            set;
        }

        [JsonPropertyName("willDelayIntervalMs")]
        public uint? WillDelayIntervalMs
        {
            get;
            set;
        }

        [JsonPropertyName("topicSubscriptions")]
        public List<SimConnectTopic> TopicSubscriptions
        {
            get;
            set;
        }

        [JsonExtensionData]
        public Dictionary<string, object> ExtensionData { get; set; }
    }
}
