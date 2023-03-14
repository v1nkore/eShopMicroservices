using MassTransit.ExtensionsDependencyInjectionIntegration;

namespace MassTransit.Contracts.Configuration
{
	public class MassTransitConfiguration
	{
		public bool IsDebug { get; set; }
		public Action<IServiceCollectionBusConfigurator>? ConfigureBusConfigurator { get; set; }
		public Action<IBusControl, IServiceProvider>? ConfigureBusControl { get; set; }
		public string? ServiceName { get; set; }
	}
}
