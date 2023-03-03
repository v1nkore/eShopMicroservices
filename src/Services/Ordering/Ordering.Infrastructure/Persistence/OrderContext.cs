using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.Persistence
{
	public class OrderContext : DbContext
	{
		public OrderContext(DbContextOptions<OrderContext> options) : base(options)
		{
		}

		public DbSet<Order> Orders { get; set; }

		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			base.ConfigureConventions(configurationBuilder);

			configurationBuilder.Properties<decimal>()
				.HavePrecision(10, 2);
		}

		public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
		{
			foreach (var entry in ChangeTracker.Entries<EntityBase>())
			{
				if (entry.State is EntityState.Added or EntityState.Modified)
				{
					OnAddOrUpdateEntity(entry.Entity);
				}
			}

			return await base.SaveChangesAsync(cancellationToken);
		}

		private void OnAddOrUpdateEntity(EntityBase entity)
		{
			if (string.IsNullOrEmpty(entity.CreatedBy))
			{
				entity.CreatedBy = "isftobs";
				entity.CreatedDate = DateTime.Now;
			}
			else
			{
				entity.LastModifiedBy = "isftobs";
				entity.LastModifiedDate = DateTime.Now;
			}
		}
	}
}