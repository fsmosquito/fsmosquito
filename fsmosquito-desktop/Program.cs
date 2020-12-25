namespace FsMosquito
{
    using ElectronNET.API;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using System.Linq;

    public class Program
    {
        public static void Main(string[] args)
        {
            switch(args.FirstOrDefault())
            {
                // If we're shimming SimConnect, invoke the entrypoint of the SimConnect Shim windows host 
                case "start-shim":
                    SimConnect.Program.Main(args);
                    break;
                // In other cases, perform a regular startup of the web host
                default:
                    // Start the Web Host
                    CreateHostBuilder(args).Build().Run();
                    break;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseElectron(args);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://0.0.0.0:5272");
                });
    }
}
