using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
	public sealed class ProductRepository : IProductRepository
	{
		private readonly ICatalogContext _context;
		public ProductRepository(ICatalogContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task<IEnumerable<Product>> GetProductsAsync()
		{
			return await _context.Products.Find(_ => true).ToListAsync();
		}

		public async Task<Product> GetProductAsync(string id)
		{
			return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
		{
			return await _context.Products.Find(p => p.Name == name).ToListAsync();
		}

		public async Task<IEnumerable<Product>> GetProductByCategoryAsync(string category)
		{
			return await _context.Products.Find(p => p.Name == category).ToListAsync();

		}

		public async Task<string> CreateProductAsync(Product product)
		{
			await _context.Products.InsertOneAsync(product);
			return product.Id;
		}

		public async Task<bool> UpdateProductAsync(Product product)
		{
			var updateResult = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);

			return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
		}

		public async Task<bool> DeleteProductAsync(string id)
		{
			var deleteResult = await _context.Products.DeleteOneAsync(p => p.Id == id);

			return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
		}
	}
}
