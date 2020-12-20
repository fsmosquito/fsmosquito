namespace FsMosquito.SimConnect
{
    using FsMosquito.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MQTTnet;

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSimConnectShim(this IServiceCollection services, HostBuilderContext hostContext)
        {
            services.Configure<FsMosquitoOptions>(hostContext.Configuration.GetSection("FSMosquito"));

            services.AddMqttControllers();
            services.AddSingleton<IFsSimConnect, FsSimConnect>();
            services.AddSingleton<ISimConnectAdapter, SimConnectMqttAdapter>();
            services.AddSingleton<IFsSimConnectMqttClient, FsSimConnectMqttClient>();
            services.AddSingleton<IApplicationMessagePublisher>((sp) =>
               {
                   var simConnectMqttClient = sp.GetRequiredService<IFsSimConnectMqttClient>();
                   return simConnectMqttClient.MqttClient;
               });
            services.AddSingleton<ISimConnectEventSource, FsMosquitoForm>();
            services.AddSingleton<FsMosquitoContext>();

            return services;
        }
    }
}
