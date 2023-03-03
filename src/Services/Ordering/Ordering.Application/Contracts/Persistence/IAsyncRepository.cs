using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System.Linq.Expressions;

namespace Ordering.Application.Contracts.Persistence
{
	public interface IAsyncRepository<T> where T : EntityBase
	{
		Task<IEnumerable<T>?> GetAsync(Expression<Func<T, bool>> predicate);
		Task<T?> GetByIdAsync(Guid id);
		Task<T> AddAsync(T entity);
		Task<Order?> UpdateAsync(T entity);
		Task<bool> DeleteAsync(Guid id);
	}
}