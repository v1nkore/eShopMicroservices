using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Data
{
	public sealed class CatalogContext : ICatalogContext
	{
		private readonly IMongoClient _client;
		private readonly IMongoDatabase _database;

		public IMongoCollection<Product> Products { get; }

		public CatalogContext(IOptions<MongoDbOptions> options)
		{
			_client = new MongoClient(options.Value.ConnectionString);
			_database = _client.GetDatabase(options.Value.DatabaseName);
		}

		public IMongoCollection<T> GetCollection<T>(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null!;
			}

			return _database.GetCollection<T>(name);
		}
	}
}
