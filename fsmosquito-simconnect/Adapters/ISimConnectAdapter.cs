namespace FsMosquito.SimConnect
{
    using System.Threading.Tasks;

    /// <summary>
    /// Adapts the functionality of SimConnect to another messaging mechanism.
    /// </summary>
    public interface ISimConnectAdapter
    {
        /// <summary>
        /// Signal the that SimConnect has been opened.
        /// </summary>
        Task SimConnectOpened();

        /// <summary>
        /// Signal the that SimConnect has been closed.
        /// </summary>
        Task SimConnectClosed();

        /// <summary>
        /// Signal the that a SimConnect topic has been updated
        /// </summary>
        Task TopicValueChanged((SimConnectTopic topic, uint objectId, object value) topicValue);
    }
}