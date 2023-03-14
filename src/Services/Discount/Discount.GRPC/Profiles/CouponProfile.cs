using AutoMapper;
using Discount.GRPC.Entities;

namespace Discount.GRPC.Profiles;

public class CouponProfile : Profile
{
	public CouponProfile()
	{
		CreateMap<Coupon, CouponResponse>();
		CreateMap<CreateDiscountCommand, Coupon>();
		CreateMap<UpdateDiscountCommand, Coupon>();
	}
}