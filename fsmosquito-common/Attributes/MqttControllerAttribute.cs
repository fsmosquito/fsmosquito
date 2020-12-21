namespace FsMosquito.Attributes
{
    using System;

    /// <summary>
    /// Indicates that a type is used to serve MQTT responses.
    /// <para>
    /// Controllers decorated with this attribute are configured with features and behavior targeted at improving the
    /// developer experience for building MQTT APIs.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class MqttControllerAttribute : Attribute
    {
    }
}