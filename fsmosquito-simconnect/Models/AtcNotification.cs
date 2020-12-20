namespace FsMosquito.SimConnect
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class AtcNotification
    {
        [JsonPropertyName("message")]
        public string Message
        {
            get;
            set;
        }

        [JsonIgnore]
        public bool Directed
        {
            get;
            set;
        }

        [JsonExtensionData]
        public IDictionary<string, JsonElement> AdditionalData { get; set; }
    }
}
