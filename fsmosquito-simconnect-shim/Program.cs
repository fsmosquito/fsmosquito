namespace FsMosquito.SimConnect
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    public static class Program
    {
        private static System.Threading.Timer _processMonitor;
        /// <summary>
        ///  The main entry point for FsMosquito Simconnect Shim.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 1 && int.TryParse(args[0], out int parentProcessId))
            {
                _processMonitor = new System.Threading.Timer((pid) =>
                {
                    var parentProcess = Process.GetProcessById(parentProcessId);
                    if (parentProcess == null || parentProcess.HasExited || !parentProcess.Responding)
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }, parentProcessId, 5000, 5000);
            } else
            {
#if !DEBUG
                // In production builds, if a parent process id has not been specified, exit.
                return;
#endif
            }

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var host = CreateHostBuilder(args).Build();
            host.Services.UseSimConnectShim();
            var ctx = host.Services.GetRequiredService<FsMosquitoContext>();

            Application.Run(ctx);
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
                services.AddSimConnectShim(hostContext);
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
