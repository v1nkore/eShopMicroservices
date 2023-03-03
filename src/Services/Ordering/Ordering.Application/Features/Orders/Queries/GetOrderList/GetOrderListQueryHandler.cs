using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;

namespace Ordering.Application.Features.Orders.Queries.GetOrderList
{
	internal class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, IReadOnlyList<OrderResponse>>
	{
		private readonly IOrderRepository _orderRepository;
		private readonly IMapper _mapper;

		public GetOrderListQueryHandler(IOrderRepository orderRepository, IMapper mapper)
		{
			_orderRepository = orderRepository;
			_mapper = mapper;
		}

		public async Task<IReadOnlyList<OrderResponse>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
		{
			var orderList = await _orderRepository.GetOrdersByUserNameAsync(request.UserName);

			return _mapper.Map<IReadOnlyList<OrderResponse>>(orderList);
		}
	}
}
