namespace FsMosquito
{
    using CliWrap;

    /// <summary>
    /// Represents a Process
    /// </summary>
    public interface IProcess
    {
        /// <summary>
        /// Gets the name of the application/process
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the command
        /// </summary>
        public Command Command
        {
            get;
        }
    }
}
