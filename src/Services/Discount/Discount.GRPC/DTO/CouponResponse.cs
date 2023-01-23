namespace Discount.GRPC.DTO;

public class CouponResponse
{
	public int Id { get; set; }
	public string ProductName { get; set; }
	public string Description { get; set; }
	public int Amount { get; set; }
}