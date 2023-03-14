using MassTransit.Contracts.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MassTransit.Contracts.Configuration
{
	public static class MassTransitConfigurator
	{
		public static void ConfigureServices(
			IServiceCollection services,
			IConfiguration configuration,
			MassTransitConfiguration? massTransitConfiguration)
		{
			if (massTransitConfiguration is null || massTransitConfiguration.IsDebug)
			{
				return;
			}

			var rabbitSection = configuration.GetSection("EventBusOptions");
			NullCheckExtensions.ThrowIfNull(rabbitSection);

			var url = rabbitSection.GetValue<string>("Url");
			url.ThrowIfNull();

			var host = rabbitSection.GetValue<string>("Host");
			host.ThrowIfNull();

			services.AddMassTransit(options =>
			{
				options.AddBus(busFactory =>
				{
					var bus = Bus.Factory.CreateUsingRabbitMq(config =>
					{
						var userName = configuration.GetValue<string>("RabbitMQ:UserName1");
						var password = configuration.GetValue<string>("RabbitMQ:Password1");
						var rabbitHost = $"rabbitmq://{url}/";
						if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
						{
							rabbitHost += host;
						}
						config.Host(rabbitHost, cfg =>
						{
							cfg.Username(userName);
							cfg.Password(password);
						});
					});
					massTransitConfiguration.ConfigureBusControl?.Invoke(bus, services.BuildServiceProvider());

					return bus;
				});

				massTransitConfiguration.ConfigureBusConfigurator?.Invoke(options);
				services.AddMassTransitHostedService();
			});
		}
	}
}
