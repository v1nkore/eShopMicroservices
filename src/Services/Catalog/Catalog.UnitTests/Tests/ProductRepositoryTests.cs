using AutoMapper;
using Catalog.API.Data.Interfaces;
using Catalog.API.DTO;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Catalog.UnitTests.FakeResults;
using Catalog.UnitTests.Helpers;
using MongoDB.Driver;
using Moq;
using Shared.Responses.ServiceResponses;
using GuidConverter = Shared.Utilities.Guid.GuidConverter;

namespace Catalog.UnitTests.Tests
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
				s.Map<ProductResponse>(It.IsAny<Product>())).Returns(expectedProduct);

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.GetProductAsync(GuidConverter.Encode(_products.First().Id));

			// assert
			Assert.NotNull(response.Value);
			Assert.Equal(response.Value!.Name, expectedProduct.Name);
			Assert.Equal(response.Value.Category, expectedProduct.Category);
			Assert.Equal(response.Value.Price, expectedProduct.Price);

			_mapperMock.VerifyMap<Product, ProductResponse>(product, Times.Once());
			_collectionMock.VerifyFindAsyncCall();
		}

		[Fact]
		public async Task GetProductAsync_ShouldReturnError_WhenItsNotFound()
		{
			// arrange
			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.GetProductAsync(GuidConverter.Encode(Guid.Empty));

			// assert
			Assert.Null(response.Value);
			Assert.NotNull(response.Error);
			Assert.Equal(ServiceResponseStatus.Failed, response.Status);
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

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.GetProductsAsync();

			// assert
			Assert.NotNull(response.Value);
			Assert.Equal(response.Value!.Count(), expectedProducts.Count);

			_mapperMock.VerifyMap<List<Product>, IReadOnlyList<ProductResponse>>(expectedProducts, Times.Once());
			_collectionMock.VerifyFindAsyncCall();
		}

		[Fact]
		public async Task GetProductsByNameAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			const string name = "Product 2";
			var expectedProducts = MongoStorage.GetProducts().Where(p => p.Name == name).ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<IReadOnlyList<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().Where(p => p.Name == name).ToList());

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.GetProductsByNameAsync(name);

			// assert
			Assert.NotNull(response.Value);
			Assert.Equal(response.Value!.Count(), expectedProducts.Count);

			_mapperMock.VerifyMap<List<Product>, IReadOnlyList<ProductResponse>>(expectedProducts, Times.Once());
			_collectionMock.VerifyFindAsyncCall();
		}

		[Fact]
		public async Task GetProductsByCategoryAsync_ShouldReturnProductList_WhenItExists()
		{
			// arrange
			const string category = "Category 5";
			var expectedProducts = MongoStorage.GetProducts().Where(p => p.Category == category).ToList();
			_products.AddRange(expectedProducts);
			_mapperMock.Setup(s =>
					s.Map<IReadOnlyList<ProductResponse>>(expectedProducts))
				.Returns(MongoStorage.GetMappedProducts().Where(p => p.Category == category).ToList());

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.GetProductByCategoryAsync(category);

			// assert
			Assert.NotNull(response);
			Assert.Equal(response.Value!.Count(), expectedProducts.Count);

			_mapperMock.VerifyMap<List<Product>, IReadOnlyList<ProductResponse>>(expectedProducts, Times.Once());
			_collectionMock.VerifyFindAsyncCall();
		}

		[Fact]
		public async Task CreateProductAsync_ShouldCreateProduct()
		{
			// arrange
			var productCommand = MongoStorage.GetOneProductCommand();
			_mapperMock.Setup(s =>
				s.Map<Product>(productCommand)).Returns(MongoStorage.GetOneMappedProductCommand());
			_collectionMock.Setup(s =>
				s.InsertOneAsync(
					It.IsAny<Product>(),
					It.IsAny<InsertOneOptions>(),
					It.IsAny<CancellationToken>()));

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.CreateProductAsync(productCommand);

			// assert
			Assert.NotNull(response);
			Assert.Equal(22, response.Value!.Length);

			_mapperMock.Verify(v => v.Map<Product>(productCommand), Times.Once);
			_collectionMock.Verify(v => v.InsertOneAsync(
				It.IsAny<Product>(),
				It.IsAny<InsertOneOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ReplaceProductAsync_ShouldReplaceProduct_WhenItsFound()
		{
			// arrange
			var productCommand = MongoStorage.GetOneProductCommand();
			_mapperMock.Setup(s =>
				s.Map<Product>(productCommand)).Returns(MongoStorage.GetOneMappedProductCommand());
			_collectionMock.Setup(s =>
					s.ReplaceOneAsync(
						It.IsAny<FilterDefinition<Product>>(),
						It.IsAny<Product>(),
						It.IsAny<ReplaceOptions>(),
						It.IsAny<CancellationToken>()))
				.ReturnsAsync(new FakeReplaceOneResult(true, 1));

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.ReplaceProductAsync(productCommand);

			// assert
			Assert.True(response.Value);

			_mapperMock.VerifyMap<ProductCommand, Product>(productCommand, Times.Once());
			_collectionMock.Verify(v => v.ReplaceOneAsync(
				It.IsAny<FilterDefinition<Product>>(),
				It.IsAny<Product>(),
				It.IsAny<ReplaceOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task ReplaceProductAsync_ShouldReturnError_WhenItsNotFound()
		{
			// arrange
			var productCommand = new ProductCommand() { Id = GuidConverter.Encode(Guid.Empty) };
			var product = new Product() { Id = Guid.Empty };
			_mapperMock.Setup(s =>
				s.Map<Product>(productCommand)).Returns(product);
			_collectionMock.Setup(s =>
					s.ReplaceOneAsync(
						It.IsAny<FilterDefinition<Product>>(),
						It.IsAny<Product>(),
						It.IsAny<ReplaceOptions>(),
						It.IsAny<CancellationToken>()))
				.ReturnsAsync(new FakeReplaceOneResult(false, 0));

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.ReplaceProductAsync(productCommand);

			// assert
			Assert.False(response.Value);
			Assert.NotNull(response.Error);
			Assert.Equal(ServiceResponseStatus.Failed, response.Status);

			_mapperMock.VerifyMap<ProductCommand, Product>(productCommand, Times.Once());
			_collectionMock.Verify(v => v.ReplaceOneAsync(
				It.IsAny<FilterDefinition<Product>>(),
				It.IsAny<Product>(),
				It.IsAny<ReplaceOptions>(),
				It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task DeleteProductAsync_ShouldDeleteProduct()
		{
			// arrange
			var deleteResult = new FakeDeleteOneResult();
			_collectionMock.Setup(s =>
					s.DeleteOneAsync(
						It.IsAny<FilterDefinition<Product>>(),
						It.IsAny<CancellationToken>()))
				.ReturnsAsync(deleteResult);

			var productRepository = new ProductRepository(_contextMock.Object, _mapperMock.Object);

			// act
			var response = await productRepository.DeleteProductAsync(GuidConverter.Encode(MongoStorage.GetOneProduct().Id));

			// assert
			Assert.True(response.Value);

			_collectionMock.Verify(v =>
				v.DeleteOneAsync(
					It.IsAny<FilterDefinition<Product>>(),
					It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}