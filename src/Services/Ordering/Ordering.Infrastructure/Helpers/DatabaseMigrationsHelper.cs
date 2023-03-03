using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Helpers
{
	public static class DatabaseMigrationsHelper
	{
		public static async Task MigrateAsync(this IServiceProvider serviceProvider)
		{
			var dbContext = serviceProvider.GetRequiredService<OrderContext>();
			if (dbContext.Database.IsSqlServer())
			{
				await dbContext.Database.MigrateAsync();
			}

			await dbContext.SeedAsync();
		}
	}
}