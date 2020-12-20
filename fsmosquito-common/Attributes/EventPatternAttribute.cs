namespace FsMosquito.Attributes
{
    using System;

    /// <summary>
    /// Subscribes to incoming events with the specified pattern
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class EventPatternAttribute : Attribute
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
        /// </summary>
        public string Pattern { get; }

        /// <inheritdoc/>
        public string Name { get; set; }
    }
}