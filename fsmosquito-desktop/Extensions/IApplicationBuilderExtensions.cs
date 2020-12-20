namespace FsMosquito
{
    using ElectronNET.API;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseFsMosquito(this IApplicationBuilder applicationBuilder)
        {
            if (HybridSupport.IsElectronActive)
            {
                var fsMosquito = applicationBuilder.ApplicationServices.GetRequiredService<IFsMosquitoApp>();
                var _ = fsMosquito.Initialize();
            }

            return applicationBuilder;
        }
    }
}
