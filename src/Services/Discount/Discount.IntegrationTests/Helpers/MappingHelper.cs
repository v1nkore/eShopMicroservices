using Discount.GRPC.Entities;

namespace Discount.IntegrationTests.Helpers
{
	public static class MappingHelper
	{
		public static CouponResponse MapCouponResponseFromCoupon(Coupon coupon)
		{
			return new CouponResponse()
			{
				Id = coupon.Id,
				ProductName = coupon.ProductName,
				Description = coupon.Description,
				Amount = coupon.Amount,
			};
		}

		public static Coupon MapCouponFromCreateDiscountCommand(CreateDiscountCommand command)
		{
			return new Coupon(command.ProductName, command.Description, command.Amount);
		}

		public static Coupon MapCouponFromUpdateDiscountCommand(UpdateDiscountCommand command)
		{
			var coupon = new Coupon(command.ProductName, command.Description, command.Amount)
			{
				Id = command.Id
			};

			return coupon;
		}
	}
}