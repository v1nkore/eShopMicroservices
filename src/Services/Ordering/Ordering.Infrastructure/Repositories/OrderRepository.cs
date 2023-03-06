using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories
{
	public class OrderRepository : IOrderRepository
	{
		private readonly OrderContext _dbContext;

		public OrderRepository(OrderContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IEnumerable<Order>?> GetAsync(Expression<Func<Order, bool>>? predicate = null)
		{
			if (predicate is not null)
			{
				return await _dbContext.Orders.Where(predicate).ToListAsync();
			}

			return await _dbContext.Orders.ToListAsync();
		}

		public async Task<Order?> GetByIdAsync(Guid id)
		{
			return await _dbContext.Orders.FirstOrDefaultAsync(order => order.Id == id);
		}

		public async Task<Order> AddAsync(Order entity)
		{
			await _dbContext.Orders.AddAsync(entity);
			await _dbContext.SaveChangesAsync();

			return entity;
		}

		public async Task<Order?> UpdateAsync(Order entity)
		{
			var order = await GetByIdAsync(entity.Id);
			if (order is not null)
			{
				_dbContext.Entry(order).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
			}

			return order;
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			var order = await GetByIdAsync(id);
			if (order is not null)
			{
				_dbContext.Orders.Remove(order);
				await _dbContext.SaveChangesAsync();

				return true;
			}

			return false;
		}

		public async Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName)
		{
			return await _dbContext.Orders
				.Where(order => string.Equals(order.UserName, userName, StringComparison.InvariantCultureIgnoreCase))
				.ToListAsync();
		}
	}
}