﻿namespace FsMosquito
{
    using ElectronNET.API;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFsMosquito(this IApplicationBuilder applicationBuilder)
        {
            // Kick off a child process of ourself, but instead in shim mode.
            var pm3 = applicationBuilder.ApplicationServices.GetRequiredService<ProcessMonitor<FsMosquitoDesktopSimConnectShim>>();
            pm3.Start();

            if (HybridSupport.IsElectronActive)
            {
                var fsMosquito = applicationBuilder.ApplicationServices.GetRequiredService<IFsMosquitoApp>();
                var _ = fsMosquito.Initialize();
            }

            return applicationBuilder;
        }
    }
}
