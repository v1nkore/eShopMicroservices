using AutoMapper;
using Catalog.API.Data.Interfaces;
using Catalog.API.DTO;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using MongoDB.Driver;
using Moq;
using Shared.Utils.Guid;

namespace Catalog.UnitTests
{
	public sealed class ProductRepositoryTests
	{
		private readonly Mock<ICatalogContext> _contextMock = new();
		private readonly Mock<IMongoCollection<Product>> _collectionMock = new();
		private readonly Mock<IAsyncCursor<Product>> _productCursorMock = new();
		private readonly Mock<IMapper> _mapperMock = new();
		private readonly List<Product> _products = new();

		public ProductRepositoryTests()
		{
			_productCursorMock.Setup(s => s.Current).Returns(_products);
			_productCursorMock.SetupSequence(s =>
				s.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
			_productCursorMock.SetupSequence(s =>
				s.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

			_contextMock.Setup(s =>
				s.GetCollection<Product>(nameof(Product))).Returns(_collectionMock.Object);

			_collectionMock.Setup(s => s.FindAsync(
					It.IsAny<FilterDefinition<Product>>(),
					It.IsAny<FindOptions<Product>>(),
					It.IsAny<CancellationToken>()))
				.ReturnsAsync(_productCursorMock.Object);
		}


		[Fact]
		public async Task GetProductAsync_ShouldReturnProduct_WhenItExists()
		{
			// arrange
			var product = MongoStorage.GetOneProduct();
			await _collectionMock.Object.InsertOneAsync(product);
			_products.Add(product);

			var expectedProduct = MongoStorage.GetOneProductResponse();
			_mapperMock.Setup(s =>
				s.Map<Product, ProductResponse>(It.IsAny<Product>())).Returns(expectedProduct);

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.GetProductAsync(GuidConverter.Encode(_products.First().Id));

			// assert
			Assert.NotNull(response);
			Assert.Equal(response.Name, expectedProduct.Name);
			Assert.Equal(response.Category, expectedProduct.Category);
			Assert.Equal(response.Price, expectedProduct.Price);

			_collectionMock.Verify(v => v.FindAsync(
				It.IsAny<FilterDefinition<Product>>(),
				It.IsAny<FindOptions<Product>>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task GetProductsAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			var expectedProducts = MongoStorage.GetProducts().ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<List<Product>, List<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().ToList());

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.GetProductsAsync();

			// assert
			Assert.NotNull(response);
			Assert.Equal(response.Count(), expectedProducts.Count);
		}


		[Fact]
		public async Task GetProductsByNameAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			var name = "Product 2";
			var expectedProducts = MongoStorage.GetProducts().Where(p => p.Name == name).ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<List<Product>, List<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().Where(p => p.Name == name).ToList());

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.GetProductsByNameAsync(name);

			// assert
			Assert.NotNull(response);
			Assert.Equal(response.Count(), expectedProducts.Count);
		}

		[Fact]
		public async Task GetProductsByCategoryAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			var category = "Category 5";
			var expectedProducts = MongoStorage.GetProducts().Where(p => p.Category == category).ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<List<Product>, List<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().Where(p => p.Category == category).ToList());

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.GetProductByCategoryAsync(category);

			// assert
			Assert.NotNull(response);
			Assert.Equal(response.Count(), expectedProducts.Count);
		}
	}
}
