using Discount.GRPC.Entities;

namespace Discount.GRPC.Repositories.Interfaces;

public interface IDiscountRepository
{
	Task<Coupon> GetDiscountAsync(string productName);

	Task<bool> CreateDiscountAsync(Coupon coupon);
	Task<bool> UpdateDiscountAsync(Coupon coupon);
	Task<bool> DeleteDiscountAsync(string productName);
}