namespace FsMosquito.Attributes
{
    using System;

    /// <summary>
    /// Indicates that a type and all derived types are used to serve MQTT responses.
    /// <para>
    /// Controllers decorated with this attribute are configured with features and behavior targeted at improving the
    /// developer experience for building APIs.
    /// </para>
    /// <para>
    /// When decorated on an assembly, all controllers in the assembly will be treated as controllers with API behavior.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MqttControllerAttribute : Attribute
    {
    }
}