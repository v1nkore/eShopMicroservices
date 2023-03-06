using AutoFixture;
using Ordering.Domain.Entities;

namespace Ordering.UnitTests.Helpers
{
	internal static class FakeStorage
	{
		private static int _index = 1;

		internal static IEnumerable<Order> GetOrders(int count)
		{
			var fixture = new Fixture()
				.Build<Order>()
				.Without(p => p.UserName);

			while (count-- > 0)
			{
				var order = fixture.Create<Order>();
				order.UserName = $"UserName {_index++}";

				yield return order;
			}
		}
	}
}
