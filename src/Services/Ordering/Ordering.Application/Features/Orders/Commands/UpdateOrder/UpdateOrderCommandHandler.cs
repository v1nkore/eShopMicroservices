using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
	public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Order?>
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IMapper _mapper;

		public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
		{
			_orderRepository = orderRepository;
			_mapper = mapper;
		}

		public async Task<Order?> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
		{
			var updatedOrder = await _orderRepository.UpdateAsync(_mapper.Map<Order>(request));
			if (updatedOrder is null)
			{
				// TODO: Log error
			}

			return updatedOrder;
		}
	}
}