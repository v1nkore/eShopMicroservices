using AutoMapper;
using Catalog.API.Data.Interfaces;
using Catalog.API.DTO;
using Catalog.API.Entities;
using Catalog.API.Guid;
using Catalog.API.Repositories.Interfaces;
using Catalog.API.ServiceResponses;
using MongoDB.Driver;

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

		public async Task<ServiceResponse<IReadOnlyList<ProductResponse>, string>> GetProductsAsync()
		{
			var products = await _context.GetCollection<Product>(nameof(Product))
				.FindAsync(_ => true)
				.Result.ToListAsync();

			return new ServiceResponse<IReadOnlyList<ProductResponse>, string>()
			{
				Value = _mapper.Map<IReadOnlyList<ProductResponse>>(products),
				Status = ServiceResponseStatus.Success
			};
		}

		public async Task<ServiceResponse<ProductResponse, string>> GetProductAsync(string id)
		{
			var product = await _context.GetCollection<Product>(nameof(Product))
				.FindAsync(p => p.Id == GuidConverter.Decode(id))
				.Result.FirstOrDefaultAsync();

			if (product is not null)
			{
				return new ServiceResponse<ProductResponse, string>()
				{
					Value = _mapper.Map<ProductResponse>(product),
					Status = ServiceResponseStatus.Success
				};
			}

			return new ServiceResponse<ProductResponse, string>()
			{
				Error = $"Product with the specified id: {id} was not found",
				Status = ServiceResponseStatus.Failed
			};
		}

		public async Task<ServiceResponse<IReadOnlyList<ProductResponse>, string>> GetProductsByNameAsync(string name)
		{
			var products = await _context.GetCollection<Product>(nameof(Product))
				.FindAsync(p => p.Name == name)
				.Result.ToListAsync();

			return new ServiceResponse<IReadOnlyList<ProductResponse>, string>()
			{
				Value = _mapper.Map<IReadOnlyList<ProductResponse>>(products),
				Status = ServiceResponseStatus.Success
			};
		}

		public async Task<ServiceResponse<IReadOnlyList<ProductResponse>, string>> GetProductByCategoryAsync(string category)
		{
			var products = await _context.GetCollection<Product>(nameof(Product))
				.FindAsync(p => p.Name == category)
				.Result.ToListAsync();

			return new ServiceResponse<IReadOnlyList<ProductResponse>, string>()
			{
				Value = _mapper.Map<IReadOnlyList<ProductResponse>>(products),
				Status = ServiceResponseStatus.Success
			};
		}

		public async Task<ServiceResponse<string, string>> CreateProductAsync(ProductCommand product)
		{
			var mapped = _mapper.Map<Product>(product);
			await _context.GetCollection<Product>(nameof(Product)).InsertOneAsync(mapped);

			return new ServiceResponse<string, string>()
			{
				Value = GuidConverter.Encode(mapped.Id),
				Status = ServiceResponseStatus.Success
			};
		}

		public async Task<ServiceResponse<bool, string>> ReplaceProductAsync(ProductCommand product)
		{
			var mapped = _mapper.Map<Product>(product);
			var replaceResult = await _context.GetCollection<Product>(nameof(Product)).ReplaceOneAsync(p => p.Id == mapped.Id, mapped);

			var success = replaceResult.IsAcknowledged && replaceResult.ModifiedCount > 0;
			var status = ServiceResponseStatus.Success;
			string? error = null;
			if (!success)
			{
				status = ServiceResponseStatus.Failed;
				error = $"Error occurred when trying to replace product with specified id: {product.Id}";
			}

			return new ServiceResponse<bool, string>()
			{
				Value = success,
				Error = error,
				Status = status,
			};
		}

		public async Task<ServiceResponse<bool, string>> DeleteProductAsync(string id)
		{
			var deleteResult = await _context.GetCollection<Product>(nameof(Product)).DeleteOneAsync(p => p.Id == GuidConverter.Decode(id));

			var success = deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
			var status = ServiceResponseStatus.Success;
			string? error = null;
			if (!success)
			{
				status = ServiceResponseStatus.Failed;
				error = $"Error occurred when trying to delete product with specified id: {id}";
			}

			return new ServiceResponse<bool, string>()
			{
				Value = success,
				Error = error,
				Status = status,
			};
		}
	}
}