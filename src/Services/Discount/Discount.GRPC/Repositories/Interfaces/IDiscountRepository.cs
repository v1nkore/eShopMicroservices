using Discount.GRPC.DTO;
using Discount.GRPC.Entities;

namespace Discount.GRPC.Repositories.Interfaces;

public interface IDiscountRepository
{
	Task<Coupon> GetDiscountAsync(string productName);

	Task<bool> CreateDiscountAsync(CouponCommand coupon);
	Task<bool> UpdateDiscountAsync(CouponCommand coupon);
	Task<bool> DeleteDiscountAsync(string productName);
}