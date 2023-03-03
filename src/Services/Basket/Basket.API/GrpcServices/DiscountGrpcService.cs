namespace Basket.API.GrpcServices
{
	public class DiscountGrpcService
	{
		private readonly DiscountProto.DiscountProtoClient _discountGrpcClient;

		public DiscountGrpcService(DiscountProto.DiscountProtoClient discountGrpcClient)
		{
			_discountGrpcClient = discountGrpcClient ?? throw new ArgumentNullException(nameof(discountGrpcClient));
		}

		public async Task<CouponResponse> GetDiscountAsync(string productName)
		{
			var discountRequest = new GetDiscountRequest() { ProductName = productName };

			return await _discountGrpcClient.GetDiscountAsync(discountRequest);
		}
	}
}