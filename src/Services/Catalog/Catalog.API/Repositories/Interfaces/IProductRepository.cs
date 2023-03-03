using Catalog.API.DTO;
using Shared.ServiceResponses;

namespace Catalog.API.Repositories.Interfaces
{
	public interface IProductRepository
	{
		Task<ServiceResponse<IReadOnlyList<ProductResponse>, string>> GetProductsAsync();
		Task<ServiceResponse<ProductResponse, string>> GetProductAsync(string id);
		Task<ServiceResponse<IReadOnlyList<ProductResponse>, string>> GetProductsByNameAsync(string name);
		Task<ServiceResponse<IReadOnlyList<ProductResponse>, string>> GetProductByCategoryAsync(string category);

		Task<ServiceResponse<string, string>> CreateProductAsync(ProductCommand product);
		Task<ServiceResponse<bool, string>> ReplaceProductAsync(ProductCommand product);
		Task<ServiceResponse<bool, string>> DeleteProductAsync(string id);
	}
}