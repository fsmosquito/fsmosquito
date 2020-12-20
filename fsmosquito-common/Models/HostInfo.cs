namespace FsMosquito
{
    using System;
    using System.Text.Json.Serialization;

    public class HostInfo
    {
        [JsonPropertyName("name")]
        public string Name
        {
            get;
            set;
        }

        [JsonPropertyName("os")]
        public string OS
        {
            get;
            set;
        }

        public static HostInfo GetHostInfo()
        {
            var result = new HostInfo()
            {
                Name = Environment.MachineName,
                OS = Environment.OSVersion.ToString(),
            };

            return result;
        }
    }
}
