namespace FsMosquito.SimConnect
{
    using CliWrap;
    using System;
    using System.IO;

    /// <summary>
    /// Represents an implementation of the IProcess that starts the FsMosquitoClient in shim mode.
    /// </summary>
    internal sealed class FsMosquitoSimConnectClientShim : IProcess
    {
        public string Name => "FsMosquito SimConnect Client Shim";

        public Command Command => Cli.Wrap("FsMosquitoClient")
                    .WithWorkingDirectory(Directory.GetCurrentDirectory())
                    .WithArguments(new string[] { "start-shim", "-s", $"-p {Environment.ProcessId}" });
    }
}
