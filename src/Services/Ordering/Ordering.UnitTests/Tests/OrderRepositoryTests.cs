using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Repositories;
using Ordering.UnitTests.Fixtures;
using Ordering.UnitTests.Helpers;

namespace Ordering.UnitTests.Tests
{
	public class OrderRepositoryTests : IClassFixture<DatabaseFixture>
	{
		private const string UserName = "UserName 1";
		private readonly OrderRepository _orderRepository;
		private readonly OrderContext _orderContext;

		public OrderRepositoryTests(DatabaseFixture databaseFixture)
		{
			_orderContext = databaseFixture.OrderContext;
			_orderRepository = new OrderRepository(_orderContext);
		}

		[Fact]
		public async Task GetAsync_ShouldReturnOrders()
		{
			// arrange

			// act
			var orders = await _orderRepository.GetAsync(order => order.UserName == UserName);

			// assert
			Assert.NotNull(orders);
			Assert.Equal(1, orders.Count());
		}

		[Fact]
		public async Task GetByIdAsync_ShouldReturnOrder_WhenItFound()
		{
			// arrange
			var orderId = (await _orderRepository.GetAsync())!.First().Id;

			// act
			var order = await _orderRepository.GetByIdAsync(orderId);

			// assert
			Assert.NotNull(order);
			Assert.Equal(order.Id, orderId);
		}

		[Fact]
		public async Task AddAsync_ShouldReturnAddedOrder()
		{
			// arrange
			var order = FakeStorage.GetOrders(1).First();
			var before = await _orderContext.Orders.CountAsync();

			// act
			var addedOrder = await _orderRepository.AddAsync(order);

			// assert
			Assert.NotNull(addedOrder);
			Assert.Equal(before + 1, await _orderContext.Orders.CountAsync());
		}

		[Fact]
		public async Task UpdateAsync_ShouldUpdateOrder_WhenItFound()
		{
			// arrange
			const string newUserName = "Updated";
			var order = _orderContext.Orders.First();
			order.UserName = newUserName;

			// act
			var updatedOrder = await _orderRepository.UpdateAsync(order);
			var orders = await _orderContext.Orders.ToListAsync();

			// assert
			Assert.NotNull(updatedOrder);
			Assert.Equal(newUserName, updatedOrder.UserName);
		}

		[Fact]
		public async Task DeleteAsync_ShouldDeleteOrder_WhenItFound()
		{
			// arrange
			var orderId = (await _orderContext.Orders.FirstAsync()).Id;
			var before = await _orderContext.Orders.CountAsync();

			// act
			var deleteResult = await _orderRepository.DeleteAsync(orderId);

			// assert
			Assert.True(deleteResult);
			Assert.Equal(before - 1, await _orderContext.Orders.CountAsync());
		}


		[Fact]
		public async Task UpdateAsync_ShouldReturnNull_WhenOrderNotFound()
		{
			// arrange
			var order = new Fixture().Build<Order>().Create();

			// act
			var updatedOrder = await _orderRepository.UpdateAsync(order);

			// assert
			Assert.Null(updatedOrder);
		}

		[Fact]
		public async Task DeleteAsync_ShouldReturnFalse_WhenOrderNotFound()
		{
			// arrange
			var orderId = Guid.Empty;

			// act
			var deleteResult = await _orderRepository.DeleteAsync(orderId);

			// assert
			Assert.False(deleteResult);
		}
	}
}
