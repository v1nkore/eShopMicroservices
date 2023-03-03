using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories.Interfaces;
using EventBus.Messages.Events;
using Grpc.Core;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class BasketController : ControllerBase
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IMapper _mapper;
		private readonly IPublishEndpoint _publishEndpoint;
		private readonly DiscountGrpcService _discountGrpcService;

		public BasketController(
			IBasketRepository basketRepository,
			IMapper mapper,
			IPublishEndpoint publishEndpoint,
			DiscountGrpcService discountGrpcService)
		{
			_basketRepository = basketRepository;
			_mapper = mapper;
			_publishEndpoint = publishEndpoint;
			_discountGrpcService = discountGrpcService;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetBasketAsync([FromQuery(Name = "u")] string userName)
		{
			var basket = await _basketRepository.GetBasketAsync(userName);

			return Ok(basket ?? new ShoppingCart(userName));
		}


		[HttpPost]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UpdateBasketAsync([FromBody] ShoppingCart basket)
		{
			var tasks = Task.WhenAll((basket.Items ?? Enumerable.Empty<ShoppingCartItem>())
				.Select(ApplyDiscountAsync).ToArray());

			try
			{
				await tasks;
			}
			catch (RpcException e)
			{
				// TODO: Log error
			}

			return Ok(await _basketRepository.UpdateBasketAsync(basket));
		}


		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteBasketAsync([FromQuery(Name = "u")] string userName)
		{
			await _basketRepository.DeleteBasketAsync(userName);

			return Ok();
		}


		[HttpPost("checkout")]
		[ProducesResponseType(StatusCodes.Status202Accepted)]
		public async Task<IActionResult> CheckoutBasketAsync([FromBody] BasketCheckout basketCheckout)
		{
			var basket = await _basketRepository.GetBasketAsync(basketCheckout.UserName);
			if (basket is null)
			{
				return BadRequest();
			}

			var basketCheckoutEvent = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
			basketCheckoutEvent.TotalPrice = basket.TotalPrice;
			await _publishEndpoint.Publish(basketCheckoutEvent);

			// might be, i should delete basket only if message was consumed
			// await _basketRepository.DeleteBasketAsync(basket.UserName);

			return Accepted();
		}

		private async Task ApplyDiscountAsync(ShoppingCartItem item)
		{
			var coupon = await _discountGrpcService.GetDiscountAsync(item.ProductName);
			item.Price -= coupon.Amount;
		}
	}
}