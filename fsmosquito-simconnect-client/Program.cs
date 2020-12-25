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

        #region CommandLine Options
        public class LaunchOptions
        {
            [Option('s', "singleton", Required = false, Default = false, HelpText = "Indicate if only a single instance is allowed.")]
            public bool Singleton { get; set; }

            [Option('p', "pid", Required = false, Default = null, HelpText = "Indicate an id of a parent process whose lifecycle should be monitored. This process will exit if the indicated process is no longer running.")]
            public int? ParentProcessId { get; set; }
        }

        [Verb("start", true, HelpText = "Starts the FsMosquito SimConnect Client in standard mode.")]
        public class StartOptions : LaunchOptions
        {
            [Option("show", Required = false, Default = true, HelpText = "Indicate if the main form should be shown on startup.")]
            public bool ShowFormOnStartup { get; set; }
        }

        [Verb("start-shim", HelpText = "Starts the FsMosquito SimConnect Client in shim mode.")]
        public class ShimOptions : LaunchOptions
        {
        }
        #endregion

        /// <summary>
        ///  The main entry point for FsMosquito Simconnect Shim.
        /// </summary>
        [STAThread]
        public static int Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Setup DI
            var host = CreateHostBuilder(args).Build();
            host.Services.UseSimConnectShim();
            var logger = host.Services.GetRequiredService<ILogger<FsMosquitoShimContext>>();

            // Parse arguments and run.
            return Parser.Default
                .ParseArguments<StartOptions, ShimOptions>(args)
                .MapResult(
                    (StartOptions opts) =>
                    {
                        var clientContext = host.Services.GetRequiredService<FsMosquitoClientContext>();
                        var logger = host.Services.GetRequiredService<ILogger<FsMosquitoClientContext>>();
                        return Launch(opts, clientContext, logger);
                    },
                    (ShimOptions opts) =>
                    {
                        var shimContext = host.Services.GetRequiredService<FsMosquitoShimContext>();
                        var logger = host.Services.GetRequiredService<ILogger<FsMosquitoShimContext>>();
                        return Launch(opts, shimContext, logger);
                    },
                    errs => 1
                );
        }

        public static int Launch(LaunchOptions options, ApplicationContext ctx, ILogger logger)
        {
            using Mutex mutex = new Mutex(false, options.ParentProcessId.HasValue ? $"Global\\{AppGuid}-{options.ParentProcessId.Value}" : $"Global\\{AppGuid}");
            if (options.Singleton)
            {
                logger.LogInformation($"Singleton mode enabled. Current Arguments: -s {options.Singleton}");
                if (!mutex.WaitOne(0, false))
                {
                    logger.LogInformation("Singleton mode is enabled. Another instance of FsMosquito SimConnect Shim is already running. Terminating.");

                    if (options is StartOptions)
                    {
                        MessageBox.Show("Another instance of FSMosquito Client is already running.");
                    }

                    Environment.Exit(0);
                }
            }

            if (options.ParentProcessId.HasValue)
            {
                logger.LogInformation($"Parent Process mode enabled. Current Arguments: -p {options.ParentProcessId}");

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
                }, options.ParentProcessId.Value, 5000, 5000);
            }

            Application.Run(ctx);

            // Norminal Shutdown
            if (_processMonitor != null)
            {
                _processMonitor.Change(Timeout.Infinite, Timeout.Infinite);
            }
            return 0;
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
                services.AddSimConnectShim<FsMosquitoShimForm>(hostContext);

                services.AddSingleton<FsMosquitoClientForm>();
                services.AddSingleton<FsMosquitoClientContext>();
                services.AddSingleton<FsMosquitoShimForm>();
                services.AddSingleton<FsMosquitoShimContext>();
                
                services.AddSingleton<FsMosquitoSimConnectClientShim>();
                services.AddSingleton<ProcessMonitor<FsMosquitoSimConnectClientShim>>();
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
