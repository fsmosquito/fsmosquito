namespace FsMosquito.SimConnect
{
    public record FsMosquitoSimConnectClientOptions
    {
        public FsMosquitoSimConnectClientOptions()
        {
            ShowWindowOnStartup = true;
        }

        /// <summary>
        /// Gets or sets a value that indicates if the main form of the FsMosquito Client should be shown on startup.
        /// </summary>
        public bool ShowWindowOnStartup { get; set; }
    }
}
