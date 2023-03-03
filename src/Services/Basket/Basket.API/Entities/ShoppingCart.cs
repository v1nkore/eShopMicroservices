namespace Basket.API.Entities;

public class ShoppingCart
{
	public string UserName { get; set; }
	public List<ShoppingCartItem>? Items { get; set; }

	public ShoppingCart(string userName)
	{
		UserName = userName;
	}

	public decimal TotalPrice => Items is not null ? Items.Sum(item => item.Price * item.Quantity) : 0;
}