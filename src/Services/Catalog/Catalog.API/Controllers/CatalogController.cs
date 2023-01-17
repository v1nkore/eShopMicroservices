using Catalog.API.Entities;
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
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetAsync([FromRoute] string id)
		{
			var product = await _productRepository.GetProductAsync(id);

			return product is null ? NotFound() : Ok(product);
		}

		[HttpGet("all")]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetAllAsync()
		{
			var products = await _productRepository.GetProductsAsync();

			return products is null ? NotFound() : Ok(products);
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<IActionResult> CreateAsync([FromBody] Product product)
		{
			var id = await _productRepository.CreateProductAsync(product);

			return Created(new Uri(
				$"{ControllerContext.ActionDescriptor.AttributeRouteInfo!.Template}" +
				$"/{ControllerContext.ActionDescriptor.AttributeRouteInfo.Name}"), id);
		}

		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateAsync([FromBody] Product product)
		{
			var result = await _productRepository.UpdateProductAsync(product);

			return result ? Ok(result) : BadRequest(product);
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteAsync([FromRoute] string id)
		{
			var result = await _productRepository.DeleteProductAsync(id);

			return result ? Ok(result) : BadRequest(id);
		}

	}
}
