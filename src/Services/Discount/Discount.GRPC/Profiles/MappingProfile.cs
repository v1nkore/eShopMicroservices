using AutoMapper;
using Discount.GRPC.Entities;

namespace Discount.GRPC.Profiles;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Coupon, CouponResponse>();
		CreateMap<CreateDiscountCommand, Coupon>();
		CreateMap<UpdateDiscountCommand, Coupon>();
	}
}