namespace Catalog.API.DTO
{
	public class ProductCommand
	{
		public string? Id { get; init; }
		public string? Name { get; init; }
		public string? Category { get; init; }
		public string? Summary { get; init; }
		public string? Description { get; init; }
		public string? ImageFile { get; init; }
		public decimal Price { get; init; }
	}
}
