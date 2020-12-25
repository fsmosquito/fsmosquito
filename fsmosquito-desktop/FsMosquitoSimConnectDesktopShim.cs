namespace FsMosquito
{
    using CliWrap;
    using System;
    using System.IO;

    /// <summary>
    /// Represents an implementation of the IProcess that starts the FsMosquitoClient in shim mode.
    /// </summary>
    internal sealed class FsMosquitoDesktopSimConnectShim : IProcess
    {
        public string Name => "FsMosquito SimConnect Desktop Shim";

        public Command Command => Cli.Wrap("fsmosquito-desktop")
                    .WithWorkingDirectory(Directory.GetCurrentDirectory())
                    .WithArguments(new string[] { "start-shim", "-s", $"-p {Environment.ProcessId}" });
    }
}
