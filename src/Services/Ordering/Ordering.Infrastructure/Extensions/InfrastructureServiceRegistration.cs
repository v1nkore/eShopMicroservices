using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Options;
using Ordering.Infrastructure.Constants;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using SendGrid.Extensions.DependencyInjection;

namespace Ordering.Infrastructure.Extensions
{
	public static class InfrastructureServiceRegistration
	{
		public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<OrderContext>(options =>
			{
				options.UseSqlServer(configuration.GetConnectionString(OrderingInfrastructureConstants.DbConnectionStringName));
			});
			services.AddSendGrid(options =>
			{
				options.ApiKey = Environment.GetEnvironmentVariable(OrderingInfrastructureConstants.SendGridKeyName);
			});

			services.AddScoped<IOrderRepository, OrderRepository>();
			services.Configure<EmailOptions>(_ => configuration.GetSection(EmailOptions.Section));
			services.AddTransient<IEmailService, IEmailService>();
		}
	}
}