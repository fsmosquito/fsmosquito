namespace FsMosquito.Extensions
{
    using FsMosquito.Routing;
    using MQTTnet;
    using MQTTnet.Client;
    using System.Collections.Generic;

    public static class IMqttClientExtensions
    {
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
