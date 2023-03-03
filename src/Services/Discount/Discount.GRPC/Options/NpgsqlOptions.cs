namespace Discount.GRPC.Options;

public class NpgsqlOptions : INpgsqlOptions
{
	public const string Section = "NpgsqlOptions";
	public string ConnectionString { get; set; } = null!;
}