using MediatR;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
	public class DeleteOrderCommand : IRequest<bool>
	{
		public Guid Id { get; set; }

		public DeleteOrderCommand(Guid id)
		{
			Id = id;
		}
	}
}