using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.API.EventBusConsumers
{
	public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
	{
		private readonly IMediator _mediator;
		private readonly IMapper _mapper;

		public BasketCheckoutConsumer(IMediator mediator, IMapper mapper)
		{
			_mediator = mediator;
			_mapper = mapper;
		}

		public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
		{
			var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
			var newOrderId = await _mediator.Send(command);

			// TODO: Log result
		}
	}
}
