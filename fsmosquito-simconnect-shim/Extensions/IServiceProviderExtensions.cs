namespace FsMosquito.SimConnect
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public static class IServiceProviderExtensions
    {
        public static IServiceProvider UseSimConnectShim(this IServiceProvider services)
        {
            var mqttClient = services.GetRequiredService<IFsSimConnectMqttClient>();
            var adapter = services.GetRequiredService<ISimConnectAdapter>();
            var simConnectEventSource = services.GetRequiredService<ISimConnectEventSource>();
            var simConnect = services.GetRequiredService<IFsSimConnect>();

            // Instruct the MQTT Client to start connecting.
            mqttClient.Connect();

            // SimConnect messages raised by the SimConnectEventSource (Forms App) to the FsSimConnect wrapper.
            simConnectEventSource.Subscribe(simConnect);

            // Subscribe SimConnect
            simConnect.SimConnectOpened += async (sender, e) =>
            {
                if (mqttClient.IsConnected)
                    await adapter.SimConnectOpened();
            };

            simConnect.SimConnectClosed += async (sender, e) =>
            {
                if (mqttClient.IsConnected)
                    await adapter.SimConnectClosed();
            };

            simConnect.TopicValueChanged += async (sender, e) =>
            {
                if (mqttClient.IsConnected)
                    await adapter.TopicValueChanged(e);
            };

            // Instruct SimConnect to connect to SimConnect using the HWnd of the SimConnectEventSource.
            simConnect.Connect(simConnectEventSource.Handle);

            return services;
        }
    }
}
