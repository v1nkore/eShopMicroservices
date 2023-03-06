using Catalog.API.DTO;
using Catalog.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Responses.ServiceResponses;

namespace Catalog.API.Controllers
{
	public sealed class CatalogController : ApiControllerBase
	{
		private readonly IProductRepository _productRepository;

		public CatalogController(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetAsync([FromRoute] string id)
		{
			var response = await _productRepository.GetProductAsync(id);

			return response.Value is null ? NotFound() : Ok(response.Value);
		}

		[HttpGet("all")]
		[ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetAllAsync()
		{
			var response = await _productRepository.GetProductsAsync();

			return response.Value is null ? NotFound(response.Error) : Ok(response.Value);
		}

		[HttpGet("name")]
		[ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByNameAsync([FromQuery(Name = "n")] string name)
		{
			return Ok(await _productRepository.GetProductsByNameAsync(name));
		}

		[HttpGet("category")]
		[ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByCategoryAsync([FromQuery(Name = "c")] string category)
		{
			return Ok(await _productRepository.GetProductByCategoryAsync(category));
		}


		[HttpPost]
		[ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
		public async Task<IActionResult> CreateAsync([FromBody] ProductCommand product)
		{
			return Ok(await _productRepository.CreateProductAsync(product));
		}

		[HttpPatch]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateAsync([FromBody] ProductCommand product)
		{
			var response = await _productRepository.ReplaceProductAsync(product);

			return response.Status == ServiceResponseStatus.Success ? Ok(response.Value) : BadRequest(response.Error);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteAsync([FromRoute] string id)
		{
			var response = await _productRepository.DeleteProductAsync(id);

			return response.Status == ServiceResponseStatus.Success ? Ok(response.Value) : BadRequest(response.Error);
		}
	}
}