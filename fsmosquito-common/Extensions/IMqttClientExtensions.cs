namespace FsMosquito.Extensions
{
    using FsMosquito.Routing;
    using MQTTnet;
    using MQTTnet.Client;
    using System.Collections.Generic;

    /// <summary>
    /// Contains extensions to IMqttClient 
    /// </summary>
    public static class IMqttClientExtensions
    {
        /// <summary>
        /// Indicates that the IMqttClient should subscribe to the topics contained in the specified router.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="router"></param>
        public static void SubscribeControllers(this IMqttClient client, MqttApplicationMessageRouter router)
        {
            var filters = new List<MqttTopicFilter>();
            foreach (var pattern in router.RouteTable.Keys)
            {
                var patternFilter = new MqttTopicFilterBuilder()
                            .WithTopic(pattern)
                            .Build();
                filters.Add(patternFilter);
            }

            client.SubscribeAsync(filters.ToArray());
        }
    }
}
