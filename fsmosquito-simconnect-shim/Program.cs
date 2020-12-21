[assembly: System.Runtime.InteropServices.Guid("38ab6e89-eb47-4146-829e-1e3d1ad86375")]

namespace FsMosquito.SimConnect
{
    using CommandLine;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    public static class Program
    {
        private static readonly string AppGuid =
                ((GuidAttribute)typeof(Program).Assembly.
                    GetCustomAttributes(typeof(GuidAttribute), false).
                        GetValue(0)).Value;

        private static System.Threading.Timer _processMonitor;

        public class Options
        {
            [Option('s', "singleton", Required = false, Default = false, HelpText = "Indicate that a only a single instance is allowed.")]
            public bool Singleton { get; set; }

            [Option('p', "pid", Required = false, Default = null, HelpText = "Indicate an id of a parent process whose lifecycle should be monitored. This process will exit if the indicated process is no longer running.")]
            public int? ParentProcessId { get; set; }
        }

        /// <summary>
        ///  The main entry point for FsMosquito Simconnect Shim.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Setup DI
            var host = CreateHostBuilder(args).Build();
            host.Services.UseSimConnectShim();
            var ctx = host.Services.GetRequiredService<FsMosquitoContext>();
            var logger = host.Services.GetRequiredService<ILogger<FsMosquitoContext>>();

            // Setup Mutex
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    using Mutex mutex = new Mutex(false, $"Global\\{AppGuid}");
                    if (o.Singleton)
                    {
                        logger.LogInformation($"Singleton mode enabled. Current Arguments: -s {o.Singleton}");
                        if (!mutex.WaitOne(0, false))
                        {
                            logger.LogInformation("Singleton mode is enabled. Another instance of FsMosquito SimConnect Shim is already running. Terminating.");
                            Environment.Exit(0);
                        }
                    }

                    if (o.ParentProcessId.HasValue)
                    {
                        logger.LogInformation($"Parent Process mode enabled. Current Arguments: -p {o.ParentProcessId}");

                        _processMonitor = new System.Threading.Timer((pid) =>
                        {
                            Process parentProcess = null;
                            try
                            {
                                parentProcess = Process.GetProcessById((int)pid);
                            }
                            catch
                            {
                            // Do Nothing
                        }


                            if (parentProcess == null || parentProcess.HasExited || !parentProcess.Responding)
                            {
                                logger.LogInformation($"Parent Process mode is enabled. The indicated parent process of FsMosquito SimConnect Shim is either null or can no longer be found. Terminating.");
                                Environment.Exit(0);
                            }
                        }, o.ParentProcessId.Value, 5000, 5000);
                    }

                    Application.Run(ctx);
                });
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureHostConfiguration(configBuilder =>
            {
                configBuilder.AddJsonFile("appsettings.json", optional: true);
                configBuilder.AddEnvironmentVariables();

                if (args != null)
                {
                    configBuilder.AddCommandLine(args);
                }
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddOptions();
                services.AddSimConnectShim<FsMosquitoForm>(hostContext);
                services.AddSingleton<FsMosquitoContext>();
            })
            .ConfigureLogging((hostingContext, logging) => {
                var targetLogsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "fsmosquito-shim-logs");
                if (!Directory.Exists(targetLogsPath))
                {
                    Directory.CreateDirectory(targetLogsPath);
                }
                logging.AddFile($"{targetLogsPath}/FsMosquitoShim-{{Date}}.txt");
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
            });
    }
}
