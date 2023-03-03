using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrderList
{
	public class GetOrderListQuery : IRequest<IReadOnlyList<OrderResponse>>
	{
		public string UserName { get; set; }

		public GetOrderListQuery(string userName)
		{
			UserName = userName ?? throw new ArgumentNullException(nameof(userName));
		}
	}
}
