namespace FsMosquito
{
    using CliWrap;
    using CliWrap.EventStream;
    using ElectronNET.API;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Program
    {
        private static readonly Command _simConnectShimCommand = Cli.Wrap("fsmosquito-desktop")
                    .WithWorkingDirectory(Directory.GetCurrentDirectory())
                    .WithArguments(new string[] { "StartSimConnectShim", "-s", $"-p {Environment.ProcessId}" });
        
        private static ILogger<Program> _logger;
        private static IDisposable _simConnectSubscription;

        public static void Main(string[] args)
        {
            switch(args.FirstOrDefault())
            {
                // If we're shimming SimConnect, invoke the entrypoint of the SimConnect Shim windows host 
                case "StartSimConnectShim":
                    SimConnect.Program.Main(args.Skip(1).ToArray());
                    break;
                // In other cases, perform a regular startup of the web host
                default:
                    var host = CreateHostBuilder(args).Build();
                    _logger = host.Services.GetRequiredService<ILogger<Program>>();

                    // Kick off a child process of ourself, but instead in shim mode.
                    StartSimConnectShim();

                    // Start the Web Host
                    host.Run();
                    break;
            }
        }

        /// <summary>
        /// Starts the SimConnectShim
        /// </summary>
        public static void StartSimConnectShim()
        {
            // If we already have a subscription, dispose of it.
            if (_simConnectSubscription != null)
            {
                _simConnectSubscription.Dispose();
                _simConnectSubscription = null;
            }

            // Observe the simconnect shim process.
            _simConnectSubscription = _simConnectShimCommand.Observe().Subscribe(new SimConnectShimObserver());
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:5272");
                });

        /// <summary>
        /// Represents an object that observes the simconnect shim.
        /// </summary>
        private sealed class SimConnectShimObserver : IObserver<CommandEvent>
        {
            public void OnCompleted()
            {
                if (_logger != null)
                {
                    _logger.LogInformation("FsMosquito SimConnect Shim Completed.");
                }
            }

            public void OnError(Exception error)
            {
                if (_logger != null)
                {
                    _logger.LogError($"An exception occurred observing the FsMosquito SimConnect Shim: {error.Message}", error);
                }

                // Restart the shim if we didn't get a clean exit.
                Task.Run(() => StartSimConnectShim());
            }

            public void OnNext(CommandEvent cmdEvent)
            {
                switch(cmdEvent)
                {
                    case StartedCommandEvent started:
                        if (_logger != null)
                        {
                            _logger.LogInformation($"FsMosquito SimConnect Shim Started. (ProcessId {started.ProcessId})");
                        }
                        break;
                    case StandardOutputCommandEvent stdOut:
                        if (_logger != null)
                        {
                            _logger.LogInformation($"FsMosquito SimConnect Shim StdOut: {stdOut.Text}");
                        }
                        break;
                    case StandardErrorCommandEvent stdErr:
                        if (_logger != null)
                        {
                            _logger.LogWarning($"FsMosquito SimConnect Shim StdErr: {stdErr.Text}");
                        }
                        break;
                    case ExitedCommandEvent exited:
                        if (_logger != null)
                        {
                            _logger.LogWarning($"FsMosquito SimConnect Shim Exited: {exited.ExitCode}");
                        }

                        // Restart the shim if we didn't get a clean exit.
                        if (exited.ExitCode != 0)
                        {
                            Task.Run(() => StartSimConnectShim());
                        }
                        break;
                    default:
                        if (_logger != null)
                        {
                            _logger.LogWarning($"FsMosquito SimConnect Shim Unknown or unsupported CommandEvent: {cmdEvent}");
                        }
                        break;
                }
            }
        }
    }
}
