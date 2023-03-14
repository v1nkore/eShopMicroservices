using AutoMapper;
using MassTransit;
using MassTransit.Contracts.Commands;
using MediatR;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.API.EventBusConsumers
{
	public class BasketCheckoutConsumer : IConsumer<BasketCheckoutCommand>
	{
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;

		public BasketCheckoutConsumer(IMediator mediator, IMapper mapper)
		{
			_mediator = mediator;
			_mapper = mapper;
		}

		public async Task Consume(ConsumeContext<BasketCheckoutCommand> context)
		{
			var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
			var newOrderId = await _mediator.Send(command);

			// TODO: Log result
		}
	}
}
