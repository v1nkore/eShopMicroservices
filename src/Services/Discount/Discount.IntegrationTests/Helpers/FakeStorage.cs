using Discount.GRPC.Entities;

namespace Discount.IntegrationTests.Helpers
{
	public static class FakeStorage
	{
		private static (string name, string description, int amount) _discountBase => ("Product", "ProductDiscount", 20);

		public static CouponResponse GetCouponResponse()
		{
			return new CouponResponse()
			{
				ProductName = _discountBase.name,
				Description = _discountBase.description,
				Amount = _discountBase.amount,
			};
		}

		public static Coupon GetCoupon()
		{
			return new Coupon(_discountBase.name, _discountBase.description, _discountBase.amount);
		}
	}
}