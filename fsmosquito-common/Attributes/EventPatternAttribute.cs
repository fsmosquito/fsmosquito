namespace FsMosquito.Attributes
{
    using System;

    /// <summary>
    /// When decorated on MQTT Controller Methods, indicates to subscribe the method to incoming MQTT messages with the specified pattern.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class EventPatternAttribute : Attribute
    {
        /// <summary>
        /// Creates a new <see cref="EventPatternAttribute"/> with the given pattern.
        /// </summary>
        /// <param name="pattern">The event topic pattern. May not be null.</param>
        public EventPatternAttribute(string pattern)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));
        }

        /// <summary>
        /// Gets the event topic pattern.
        /// Supports {MachineName} and {HostName} tokens - more tokens may be added over time.
        /// </summary>
        public string Pattern { get; }

        /// <inheritdoc/>
        public string Name { get; set; }
    }
}