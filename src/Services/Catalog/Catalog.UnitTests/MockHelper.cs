﻿using Catalog.API.Data;
using Catalog.API.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace Catalog.UnitTests
{
	internal static class MockHelper
	{
		private static readonly Mock<IOptions<MongoDbOptions>> OptionsMock = new();
		private static readonly Mock<IMongoDatabase> MongoDatabaseMock = new();
		private static readonly Mock<IMongoClient> MongoClientMock = new();
		private static readonly Mock<IMongoCollection<Product>> CollectionMock = new();

		public static MongoDbOptions Options => new()
		{
			ConnectionString = "mongodb://testCon",
			DatabaseName = "testDb",
			CollectionName = "testCollection",
		};
	}
}