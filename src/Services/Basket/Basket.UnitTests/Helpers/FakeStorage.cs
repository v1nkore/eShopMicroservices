using Basket.API.Entities;

namespace Basket.UnitTests.Helpers
{
	internal static class FakeStorage
	{
		private const string UserName = "UserName";

		public static ShoppingCart GetShoppingCart()
		{
			var basket = new ShoppingCart(UserName);
			basket.Items = new List<ShoppingCartItem>()
			{
				new ShoppingCartItem() { Price = 100, ProductName = "Product", Quantity = 5}
			};

			return basket;
		}

		public static ShoppingCart GetUpdatedShoppingCart()
		{
			var basket = new ShoppingCart(UserName);
			basket.Items = new List<ShoppingCartItem>()
			{
				new ShoppingCartItem() { Price = 200, ProductName = "Updated product", Quantity = 22 }
			};

			return basket;
		}
	}
}
