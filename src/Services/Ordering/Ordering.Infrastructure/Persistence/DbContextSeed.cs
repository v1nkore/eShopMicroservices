using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
	public static class DbContextSeed
	{
		public static async Task SeedAsync(this OrderContext dbContext)
		{
			if (!await dbContext.Orders.AnyAsync())
			{
				var order = new Order
				{
					UserName = "isftobs",
					EmailAddress = "example8080@gmail.com",
					Country = "Country",
					TotalPrice = 500m
				};

				await dbContext.Orders.AddAsync(order);
				await dbContext.SaveChangesAsync();
			}
		}
	}
}