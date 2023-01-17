using Catalog.API.Data.Interfaces;
using Catalog.API.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Data
{
	public sealed class CatalogContext : ICatalogContext
	{
		public IMongoCollection<Product> Products { get; }

		public CatalogContext(IOptions<MongoDbOptions> mongoDbSettings)
		{
			var client = new MongoClient(mongoDbSettings.Value.ConnectionString);
			var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
			Products = database.GetCollection<Product>(mongoDbSettings.Value.CollectionName);
		}
	}
}
