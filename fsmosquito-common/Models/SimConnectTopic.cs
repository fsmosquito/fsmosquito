namespace FsMosquito
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Represents a subscription to a SimConnect Simulation Variable.
    /// <a href="https://www.prepar3d.com/SDKv5/LearningCenter.php">Example</a>
    /// </summary>
    public record SimConnectTopic
    {
        /// <summary>
        /// Gets or sets the name of the SimConnect simulation variable.
        /// Various SimConnect implementations have differing simulation variable names
        /// See: <a href="https://www.prepar3d.com/SDKv5/sdk/references/variables/simulation_variables.html">Prepar3d Simulation Variables</a>
        /// <a href="file:///C:/MSFS%20SDK/Documentation/04-Developer_Tools/SimConnect/SimConnect_Status_of_Simulation_Variables.html">MSFS2020 Simulation Variables (Link requires the MSFS2020 SDK to be installed.)</a>
        /// </summary>
        [JsonPropertyName("datumName")]
        public string DatumName
        {
            get;
            set;
        }

        /// <summary>
        /// If specified, indicates the topic level name that will be used when publishing datum items of this topic.
        /// By default if a value for this property is not specified, the snake-cased datum name is used.
        /// However, in certain situations conflicts may arise or customizing the topic name is desirable.
        /// For instance, the default message topic for a datum named 'INDICATED ALTITUDE might be 'fsm/atc/{HostName}/obj/{objectId}/indicated_altitude'.
        /// If the value of this property is set to 'foo_bar_baz', the message topic will be 'fsm/atc/{HostName}/obj/{objectId}/foo_bar_baz'.
        /// Any best practices for topic naming for the underlying messaging provider should be respected (E.g. see <a href="https://www.hivemq.com/blog/mqtt-essentials-part-5-mqtt-topics-best-practices/">MQTT Topics & best practices</a>
        /// </summary>
        [JsonPropertyName("topicLevelName")]
        public string TopicLevelName
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
