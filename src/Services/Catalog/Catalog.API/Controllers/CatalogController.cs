using Catalog.API.DTO;
using Catalog.API.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
			var product = await _productRepository.GetProductAsync(id);

			return product is null ? NotFound() : Ok(product);
		}

		[HttpGet("all")]
		[ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetAllAsync()
		{
			var products = await _productRepository.GetProductsAsync();

			return products is null ? NotFound() : Ok(products);
		}

		[HttpGet("name")]
		[ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByNameAsync([FromQuery(Name = "n")] string name)
		{
			var products = await _productRepository.GetProductsByNameAsync(name);

			return products is null ? NotFound() : Ok(products);
		}

		[HttpGet("category")]
		[ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetByCategoryAsync([FromQuery(Name = "c")] string category)
		{
			var products = await _productRepository.GetProductByCategoryAsync(category);

			return products is null ? NotFound() : Ok(products);
		}


		[HttpPost]
		[ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
		public async Task<IActionResult> CreateAsync([FromBody] ProductCommand product)
		{
			var id = await _productRepository.CreateProductAsync(product);

			return Ok(id);
		}

		[HttpPatch]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateAsync([FromBody] ProductCommand product)
		{
			var result = await _productRepository.ReplaceProductAsync(product);

			return result ? Ok(result) : BadRequest(product);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteAsync([FromRoute] string id)
		{
			var result = await _productRepository.DeleteProductAsync(id);

			return result ? Ok(result) : BadRequest(id);
		}
	}
}
