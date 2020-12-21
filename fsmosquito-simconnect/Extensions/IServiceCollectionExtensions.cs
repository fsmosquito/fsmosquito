namespace FsMosquito.SimConnect
{
    using FsMosquito.Extensions;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using MQTTnet;

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSimConnectShim<T>(this IServiceCollection services, HostBuilderContext hostContext)
            where T : class, ISimConnectEventSource
        {
            services.Configure<FsMosquitoOptions>(hostContext.Configuration.GetSection("FSMosquito"));

            services.AddMqttControllers();
            services.AddSingleton<ISimConnect, FsSimConnect>();
            services.AddSingleton<ISimConnectAdapter, SimConnectMqttAdapter>();
            services.AddSingleton<ISimConnectMqttClient, SimConnectMqttClient>();
            services.AddSingleton<IApplicationMessagePublisher>((sp) =>
               {
                   var simConnectMqttClient = sp.GetRequiredService<ISimConnectMqttClient>();
                   return new OnlyWhenConnectedApplicationMessagePublisher(simConnectMqttClient.MqttClient);
               });
            services.AddSingleton<ISimConnectEventSource, T>();

            return services;
        }
    }
}
