using Catalog.API.Data.Interfaces;

namespace Catalog.API.Data
{
	public sealed class MongoDbOptions : IMongoDbOptions
	{
		public const string Section = "DatabaseSettings";
		public string ConnectionString { get; set; } = null!;
		public string DatabaseName { get; set; } = null!;
		public string CollectionName { get; set; } = null!;
	}
}
