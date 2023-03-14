using AutoMapper;
using Basket.API.Entities;
using MassTransit.Contracts.Commands;

namespace Basket.API.Profiles
{
	public class BasketProfile : Profile
	{
		public BasketProfile()
		{
			CreateMap<BasketCheckout, BasketCheckoutCommand>();
		}
	}
}