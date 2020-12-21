namespace FsMosquito
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents information about a host.
    /// </summary>
    public record HostInfo
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

        /// <summary>
        /// Factory method to retrieve the host info of the current host.
        /// </summary>
        /// <returns></returns>
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
