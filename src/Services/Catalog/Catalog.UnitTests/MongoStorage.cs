using Catalog.API.DTO;
using Catalog.API.Entities;
using Shared.Utilities.Guid;

namespace Catalog.UnitTests
{
	internal static class MongoStorage
	{
		public static IEnumerable<Product> GetProducts()
		{
			var index = 1;
			yield return new Product()
			{
				Id = Guid.Parse("bf9c9611-86ad-44ba-92ef-7f9959437321"),
				Name = $"Product {index}",
				Category = $"Category {index}",
				Price = 100M * index++
			};
			yield return new Product()
			{
				Id = Guid.Parse("27f7c8f3-b859-4ed4-890c-54300189c3ba"),
				Name = $"Product {index}",
				Category = $"Category {index}",
				Price = 100M * index++
			};
			yield return new Product()
			{
				Id = Guid.Parse("1d68e002-5454-4735-bada-3311d19ca4de"),
				Name = $"Product {index}",
				Category = $"Category {index}",
				Price = 100M * index++
			};
			yield return new Product()
			{
				Id = Guid.Parse("47e9e57a-f835-4cef-a8d0-9e0e426d1ca9"),
				Name = $"Product {index}",
				Category = $"Category {index}",
				Price = 100M * index++
			};
			yield return new Product()
			{
				Id = Guid.Parse("84ceea7b-3796-4b16-9d21-e41569aa7751"),
				Name = $"Product {index}",
				Category = $"Category {index}",
				Price = 100M * index++
			};
		}

		public static IEnumerable<ProductResponse> GetMappedProducts()
		{
			var products = GetProducts();
			foreach (var product in products)
			{
				yield return new ProductResponse()
				{
					Id = GuidConverter.Encode(product.Id),
					Name = product.Name,
					Category = product.Category,
					Price = product.Price
				};
			}
		}

		public static Product GetOneProduct()
		{
			return GetProducts().First();
		}

		public static ProductResponse GetOneProductResponse()
		{
			return GetMappedProducts().First();
		}

		public static ProductCommand GetOneProductCommand()
		{
			return new ProductCommand()
			{
				Name = "Product command name",
				Category = "Product command category",
				Price = 2000M,
			};
		}

		public static Product GetOneMappedProductCommand()
		{
			return new Product()
			{
				Name = "Product command name",
				Category = "Product command category",
				Price = 2000M,
			};
		}
	}
}