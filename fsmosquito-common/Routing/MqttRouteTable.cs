namespace FsMosquito.Routing
{
    using MQTTnet;
    using FsMosquito.Extensions;
    using System.Collections.Generic;
    using System.Reflection;

    public class MqttRouteTable : Dictionary<string, List<MethodInfo>>
    {
        public MethodInfo[] GetTopicSubscriptions(MqttApplicationMessage message)
        {
            var result = new List<MethodInfo>();
            foreach (var pattern in Keys)
            {
                if (message.IsTopicMatch(pattern))
                {
                    result.AddRange(this[pattern]);
                }
            }

            return result.ToArray();
        }
    }
}