using Catalog.API.DTO;

namespace Catalog.API.Repositories.Interfaces
{
	public interface IProductRepository
	{
		Task<List<ProductResponse>> GetProductsAsync();
		Task<ProductResponse> GetProductAsync(string id);
		Task<IEnumerable<ProductResponse>> GetProductsByNameAsync(string name);
		Task<IEnumerable<ProductResponse>> GetProductByCategoryAsync(string category);

		Task<string> CreateProductAsync(ProductCommand product);
		Task<bool> UpdateProductAsync(ProductCommand product);
		Task<bool> DeleteProductAsync(string id);
	}
}
