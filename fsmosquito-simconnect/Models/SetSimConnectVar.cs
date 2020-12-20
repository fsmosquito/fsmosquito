namespace FsMosquito.SimConnect
{
    using System.Text.Json.Serialization;

    public record SetSimConnectVarRequest
    {
        [JsonPropertyName("objectId")]
        public uint? ObjectId
        {
            get;
            set;
        }

        [JsonPropertyName("value")]
        public object Value
        {
            get;
            set;
        }
    }
}
