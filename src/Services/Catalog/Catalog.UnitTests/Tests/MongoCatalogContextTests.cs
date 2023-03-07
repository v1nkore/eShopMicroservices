using Catalog.API.Data;
using Catalog.API.Entities;
using Catalog.UnitTests.Helpers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace Catalog.UnitTests.Tests
{
	public sealed class MongoCatalogContextTests
	{
		private readonly Mock<IOptions<MongoDbOptions>> _optionsMock;
		private readonly Mock<IMongoDatabase> _mongoDatabaseMock;
		private readonly Mock<IMongoClient> _mongoClientMock;
		private readonly Mock<IMongoCollection<Product>> _collectionMock;

		public MongoCatalogContextTests()
		{
			_optionsMock = new Mock<IOptions<MongoDbOptions>>();
			_mongoDatabaseMock = new Mock<IMongoDatabase>();
			_mongoClientMock = new Mock<IMongoClient>();
			_collectionMock = new Mock<IMongoCollection<Product>>();
		}

		[Fact]
		public void MongoCatalogContext_ShouldCreateCatalogContext()
		{
			//arrange
			var options = MockHelper.Options;
			_optionsMock.Setup(s => s.Value).Returns(options);
			_mongoClientMock.Setup(s =>
				s.GetDatabase(_optionsMock.Object.Value.DatabaseName, null))
				.Returns(_mongoDatabaseMock.Object);
			_mongoDatabaseMock.Setup(s =>
				s.GetCollection<Product>(_optionsMock.Object.Value.CollectionName, null))
				.Returns(_collectionMock.Object);

			//act
			var context = new CatalogContext(_optionsMock.Object);

			//assert
			Assert.NotNull(context);
		}
	}
}
