using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.DeleteOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderList;

namespace Ordering.API.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class OrderController : ControllerBase
	{
		private readonly IMediator _mediator;

		public OrderController(IMediator mediator)
		{
			_mediator = mediator;
		}


		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status201Created)]
		public async Task<IActionResult> CheckoutOrderAsync([FromBody] CheckoutOrderCommand command)
		{
			return Ok(await _mediator.Send(command));
		}

		[HttpGet("{userName}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetAsync([FromRoute] string userName)
		{
			var query = new GetOrderListQuery(userName);

			return Ok(await _mediator.Send(query));
		}

		[HttpPatch]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> UpdateAsync([FromBody] UpdateOrderCommand command)
		{
			var updatedOrder = await _mediator.Send(command);

			return updatedOrder is not null ? Ok(updatedOrder) : NotFound();
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteAsync(Guid id)
		{
			var command = new DeleteOrderCommand(id);
			var result = await _mediator.Send(command);

			return result ? Ok() : NotFound();
		}

	}
}
