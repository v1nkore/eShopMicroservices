namespace Catalog.API.Data.Interfaces
{
	public interface IMongoDbOptions
	{
		public string ConnectionString { get; set; }
		public string DatabaseName { get; set; }
		public string CollectionName { get; set; }
	}
}
