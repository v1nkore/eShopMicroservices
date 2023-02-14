using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Discount.IntegrationTests.Extensions
{
	public static class WebHostBuilderExtensions
	{
		public static void OverrideServices(this IWebHostBuilder builder, Action<IServiceCollection> configure)
		{
			builder.ConfigureTestServices(configure);
		}
	}
}