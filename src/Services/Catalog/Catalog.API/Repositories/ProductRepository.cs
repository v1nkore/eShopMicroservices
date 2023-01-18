using AutoMapper;
using Catalog.API.Data.Interfaces;
using Catalog.API.DTO;
using Catalog.API.Entities;
using Catalog.API.Repositories.Interfaces;
using MongoDB.Driver;
using Shared.Utils.Guid;

namespace Catalog.API.Repositories
{
	public sealed class ProductRepository : IProductRepository
	{
		private readonly ICatalogContext _context;
		private readonly IMapper _mapper;

		public ProductRepository(ICatalogContext context, IMapper mapper)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

		public async Task<List<ProductResponse>> GetProductsAsync()
		{
			var products = await _context.Products.Find(_ => true).ToListAsync();

			return _mapper.Map<List<Product>, List<ProductResponse>>(products);
		}

		public async Task<ProductResponse> GetProductAsync(string id)
		{
			var product = await _context.Products.Find(p => p.Id == GuidConverter.Decode(id)).FirstOrDefaultAsync();

			return _mapper.Map<Product, ProductResponse>(product);
		}

		public async Task<IEnumerable<ProductResponse>> GetProductsByNameAsync(string name)
		{
			var products = await _context.Products.Find(p => p.Name == name).ToListAsync();

			return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponse>>(products);
		}

		public async Task<IEnumerable<ProductResponse>> GetProductByCategoryAsync(string category)
		{
			var products = await _context.Products.Find(p => p.Name == category).ToListAsync();

			return _mapper.Map<IEnumerable<Product>, IEnumerable<ProductResponse>>(products);

		}

		public async Task<string> CreateProductAsync(ProductCommand product)
		{
			var mapped = _mapper.Map<ProductCommand, Product>(product);
			await _context.Products.InsertOneAsync(mapped);

			return GuidConverter.Encode(mapped.Id);
		}

		public async Task<bool> UpdateProductAsync(ProductCommand product)
		{
			var mapped = _mapper.Map<ProductCommand, Product>(product);
			var updateResult = await _context.Products.ReplaceOneAsync(p => p.Id == mapped.Id, mapped);

			return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
		}

		public async Task<bool> DeleteProductAsync(string id)
		{
			var deleteResult = await _context.Products.DeleteOneAsync(p => p.Id == GuidConverter.Decode(id));

			return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
		}
	}
}
