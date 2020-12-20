namespace FsMosquito.Extensions
{
    using FsMosquito.Routing;
    using Microsoft.Extensions.DependencyInjection;
    using MQTTnet.Server;
    using System;

    public static class MqttServerOptionsBuilderExtensions
    {
        public static MqttServerOptionsBuilder WithSubscriptions(this MqttServerOptionsBuilder builder, IServiceProvider applicationServices)
        {
            var router = applicationServices.GetRequiredService<MqttApplicationMessageRouter>();
            builder.WithApplicationMessageInterceptor(router);
            return builder;
        }
    }
}
