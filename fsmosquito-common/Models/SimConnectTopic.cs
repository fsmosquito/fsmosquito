namespace FsMosquito
{
    using System.Text.Json.Serialization;

    public record SimConnectTopic
    {
        [JsonPropertyName("datumName")]
        public string DatumName
        {
            get;
            set;
        }

        [JsonPropertyName("units")]
        public string Units
        {
            get;
            set;
        }
    }
}
