using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Persistence;
using Ordering.UnitTests.Helpers;

namespace Ordering.UnitTests.Fixtures
{
	public class DatabaseFixture : IDisposable
	{
		private const string InMemoryDatabaseName = "Ordering Unit Tests";

		public OrderContext OrderContext { get; set; }

		public DatabaseFixture()
		{
			var options = new DbContextOptionsBuilder<OrderContext>()
				.UseInMemoryDatabase(InMemoryDatabaseName)
				.Options;
			OrderContext = new OrderContext(options);
			OrderContext.Orders.AddRange(FakeStorage.GetOrders(5));
			OrderContext.SaveChanges();
		}

		public void Dispose()
		{

		}
	}
}
