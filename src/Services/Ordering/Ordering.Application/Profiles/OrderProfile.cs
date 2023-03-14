using AutoMapper;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Application.Features.Orders.Commands.UpdateOrder;
using Ordering.Application.Features.Orders.Queries.GetOrderList;
using Ordering.Domain.Entities;

namespace Ordering.Application.Profiles
{
	public class OrderProfile : Profile
	{
		public OrderProfile()
		{
			CreateMap<CheckoutOrderCommand, Order>();
			CreateMap<UpdateOrderCommand, Order>();
			CreateMap<Order, OrderResponse>();
		}
	}
}