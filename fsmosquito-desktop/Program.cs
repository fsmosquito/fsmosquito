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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    public class Program
    {
        private static ILogger<Program> _logger;
        private static Command _shimCommand;
        private static IDisposable _shimSubscription;
        private static int _currentProcessId;

        public static void Main(string[] args)
        {
            // If we're shimming SimConnect, invoke the entrypoint of the SimConnect Shim. Otherwise, perform a regular startup.
            if (args != null && args.Length > 0 && args[0] == "StartShim")
            {
                SimConnect.Program.Main(args.Skip(1).ToArray());
            }
            else
            {
                var currentProcess = Process.GetCurrentProcess();
                currentProcess.EnableRaisingEvents = true;
                _currentProcessId = currentProcess.Id;

                var host = CreateHostBuilder(args).Build();
                _logger = host.Services.GetRequiredService<ILogger<Program>>();

                _shimCommand = Cli.Wrap("fsmosquito-desktop")
                    .WithWorkingDirectory(Directory.GetCurrentDirectory())
                    .WithArguments(new string[] { "StartShim", $"{_currentProcessId}" });

                // Kick off a child process of ourself, but instead in shim mode.
                StartShim();
                host.Run();
            }
        }

        public static void StartShim()
        {
            // If we already have a subscription, dispose of it.
            if (_shimSubscription != null)
            {
                _shimSubscription.Dispose();
                _shimSubscription = null;
            }

            // Observe the shim process.
            _shimSubscription = _shimCommand.Observe().Subscribe(new ShimObserver());
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:5272");
                });

        private class ShimObserver : IObserver<CommandEvent>
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
                        Console.WriteLine(stdOut.Text);
                        break;
                    case StandardErrorCommandEvent stdErr:
                        if (_logger != null)
                        {
                            _logger.LogWarning($"FsMosquito SimConnect Shim StdErr: {stdErr.Text}");
                        }
                        Console.WriteLine(stdErr.Text);
                        break;
                    case ExitedCommandEvent exited:
                        if (_logger != null)
                        {
                            _logger.LogWarning($"FsMosquito SimConnect Shim Exited: {exited.ExitCode}");
                        }
                        Console.WriteLine($"FsMosquito SimConnect Shim Exited {exited.ExitCode}");

                        // Restart the shim if we didn't get a clean exit.
                        if (exited.ExitCode != 0)
                        {
                            Task.Run(() => StartShim());
                        }
                        break;
                }
            }
        }
    }
}
