using AutoMapper;
using MassTransit.Contracts.Commands;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.API.Profiles
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<CheckoutOrderCommand, BasketCheckoutCommand>();
		}
	}
}
