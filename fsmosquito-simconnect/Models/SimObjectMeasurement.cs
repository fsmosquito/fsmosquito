namespace FsMosquito.SimConnect
{
    using System.Text.Json.Serialization;

    public record SimObjectMeasurement
    {
        [JsonPropertyName("units")]
        public string Units
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
