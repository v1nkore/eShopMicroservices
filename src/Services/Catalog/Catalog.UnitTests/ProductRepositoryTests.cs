using AutoMapper;
using Catalog.API.Data.Interfaces;
using Catalog.API.DTO;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Catalog.UnitTests.FakeResults;
using MongoDB.Driver;
using Moq;
using GuidConverter = Shared.Guid.GuidConverter;

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
			Assert.NotNull(response.Value);
			Assert.Equal(response.Value!.Name, expectedProduct.Name);
			Assert.Equal(response.Value.Category, expectedProduct.Category);
			Assert.Equal(response.Value.Price, expectedProduct.Price);

			_collectionMock.VerifyFindAsyncCall();
		}

		[Fact]
		public async Task GetProductsAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			var expectedProducts = MongoStorage.GetProducts().ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<IReadOnlyList<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().ToList());

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.GetProductsAsync();

			// assert
			Assert.NotNull(response.Value);
			Assert.Equal(response.Value!.Count(), expectedProducts.Count);

			_collectionMock.VerifyFindAsyncCall();
		}


		[Fact]
		public async Task GetProductsByNameAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			var name = "Product 2";
			var expectedProducts = MongoStorage.GetProducts().Where(p => p.Name == name).ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<IReadOnlyList<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().Where(p => p.Name == name).ToList());

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.GetProductsByNameAsync(name);

			// assert
			Assert.NotNull(response.Value);
			Assert.Equal(response.Value!.Count(), expectedProducts.Count);

			_collectionMock.VerifyFindAsyncCall();
		}

		[Fact]
		public async Task GetProductsByCategoryAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			var category = "Category 5";
			var expectedProducts = MongoStorage.GetProducts().Where(p => p.Category == category).ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<IReadOnlyList<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().Where(p => p.Category == category).ToList());

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.GetProductByCategoryAsync(category);

			// assert
			Assert.NotNull(response);
			Assert.Equal(response.Value!.Count(), expectedProducts.Count);
		}


		[Fact]
		public async Task CreateProductAsync_ShouldCreateProduct_Success()
		{
			// arrange
			var productCommand = MongoStorage.GetOneProductCommand();
			_mapperMock.Setup(s =>
				s.Map<ProductCommand, Product>(productCommand)).Returns(MongoStorage.GetOneMappedProductCommand());
			_collectionMock.Setup(s =>
				s.InsertOneAsync(
					It.IsAny<Product>(),
					It.IsAny<InsertOneOptions>(),
					It.IsAny<CancellationToken>()));

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.CreateProductAsync(productCommand);

			// assert
			Assert.NotNull(response);
			Assert.Equal(22, response.Value!.Length);

			_collectionMock.Verify(v => v.InsertOneAsync(
				It.IsAny<Product>(),
				It.IsAny<InsertOneOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}


		[Fact]
		public async Task ReplaceProductAsync_ShouldUpdateProduct_Success()
		{
			// arrange
			var productCommand = MongoStorage.GetOneProductCommand();
			var replaceResult = new FakeReplaceOneResult();
			_mapperMock.Setup(s =>
				s.Map<ProductCommand, Product>(productCommand)).Returns(MongoStorage.GetOneMappedProductCommand());
			_collectionMock.Setup(s =>
					s.ReplaceOneAsync(
						It.IsAny<FilterDefinition<Product>>(),
						It.IsAny<Product>(),
						It.IsAny<ReplaceOptions>(),
						It.IsAny<CancellationToken>()))
				.ReturnsAsync(replaceResult);

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.ReplaceProductAsync(productCommand);

			// assert
			Assert.True(response.Value);

			_collectionMock.Verify(v => v.ReplaceOneAsync(
				It.IsAny<FilterDefinition<Product>>(),
				It.IsAny<Product>(),
				It.IsAny<ReplaceOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}


		[Fact]
		public async Task DeleteProductAsync_ShouldDeleteProduct_Success()
		{
			// arrange
			var deleteResult = new FakeDeleteOneResult();
			_collectionMock.Setup(s =>
					s.DeleteOneAsync(
						It.IsAny<FilterDefinition<Product>>(),
						It.IsAny<CancellationToken>()))
				.ReturnsAsync(deleteResult);

			var productRepo = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepo.DeleteProductAsync(GuidConverter.Encode(MongoStorage.GetOneProduct().Id));

			// assert
			Assert.True(response.Value);

			_collectionMock.Verify(v =>
				v.DeleteOneAsync(
					It.IsAny<FilterDefinition<Product>>(),
					It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}