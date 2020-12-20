namespace FsMosquito.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using FsMosquito.Routing;
    using System.Reflection;
    using System.Collections.Generic;
    using Microsoft.Extensions.Options;

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddMqttControllers(this IServiceCollection services, ICollection<Assembly> assemblies = null)
        {
            if (assemblies == null)
            {
                assemblies = new Assembly[] {
                    Assembly.GetEntryAssembly()
                };
            }

            services.AddSingleton( (serviceProvider) => {
                var options = serviceProvider.GetService<IOptions<FsMosquitoOptions>>();
                return MqttRouteTableFactory.Create(assemblies, options?.Value);
            });
            services.AddSingleton<ITypeActivatorCache>(new TypeActivatorCache());
            services.AddSingleton<MqttApplicationMessageRouter>();

            return services;
        }
    }
}