using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
	public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
	{
		private readonly IOrderRepository _orderRepository;

		public DeleteOrderCommandHandler(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}

		public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
		{
			if (!await _orderRepository.DeleteAsync(request.Id))
			{
				// TODO: Log error
				return false;
			}

			return true;
		}
	}
}