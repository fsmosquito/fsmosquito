namespace FsMosquito.Extensions
{
    using MQTTnet;
    using System;
    using System.Linq;

    public static class MqttApplicationMessageExtensions
    {
        private static readonly char[] Separator = new[] { '/' };

        public static string GetTopicSegment(this MqttApplicationMessage message, int index)
        {
            return GetTopicSegments(message)
                .ElementAtOrDefault(index);
        }

        /// <summary>
        /// Returns a value that indicates if the topic in the message matches the specified patterns
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsTopicMatch(this MqttApplicationMessage message, string pattern, bool ignoreCase = false)
        {
            var topicSegments = GetTopicSegments(message.Topic);
            var patternSegments = GetTopicSegments(pattern);

            if (topicSegments.Length != patternSegments.Length)
            {
                return false;
            }

            for(int i = 0; i < topicSegments.Length; i++)
            {
                var topicSegment = topicSegments[i];
                var patternSegment = patternSegments[i];

                if (patternSegment == "+")
                {
                    continue;
                }

                if (string.Compare(topicSegment, patternSegment, ignoreCase) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static string[] GetTopicSegments(this MqttApplicationMessage message)
        {
            return GetTopicSegments(message.Topic);

        }

        private static string[] GetTopicSegments(string topic)
        {
            return topic.Trim('/')
                .Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
