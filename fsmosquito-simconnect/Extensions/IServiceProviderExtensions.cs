namespace FsMosquito.SimConnect
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class IServiceProviderExtensions
    {
        public static IServiceProvider UseSimConnectShim(this IServiceProvider services)
        {
            var mqttClient = services.GetRequiredService<ISimConnectMqttClient>();
            var adapter = services.GetRequiredService<ISimConnectAdapter>();
            var simConnectEventSource = services.GetRequiredService<ISimConnectEventSource>();
            var simConnect = services.GetRequiredService<ISimConnect>();

            // Instruct the MQTT Client to start connecting.
            mqttClient.Connect();

            // SimConnect messages raised by the SimConnectEventSource (Forms App) to the FsSimConnect wrapper.
            simConnectEventSource.Subscribe(simConnect);

            // Publish events produced by SimConnect as MQTT messages.
            simConnect.Subscribe(adapter);

            // Instruct SimConnect to connect to SimConnect using the HWnd of the SimConnectEventSource.
            simConnect.Connect(simConnectEventSource.Handle);

            return services;
        }
    }
}
