using System.Reflection;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Play.Common.Settings;

namespace Play.Common.MassTransit {
    public static class Extensions {
        public static WebApplicationBuilder AddMassTransitWithRabbitMQ(this WebApplicationBuilder builder) {
            builder.Services.AddMassTransit(configure => {
                configure.AddConsumers(Assembly.GetEntryAssembly());
                configure.UsingRabbitMq((context, configurator) => {
                    var configuration = builder.Configuration;
                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    var host = EnvironmentDeterminer.IsRunningInContainer ? rabbitMQSettings.DockerHost : rabbitMQSettings.Host;
                    configurator.Host(host);
                    configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                    configurator.UseMessageRetry(retryConfigurator => {
                        retryConfigurator.Interval(3, TimeSpan.FromSeconds(5));
                    });
                });
            });

            return builder;
        }
    } 
}