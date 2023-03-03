namespace Ordering.Application.Options
{
	public class EmailOptions
	{
		public const string Section = "EmailOptions";
		public string ApiKey { get; set; } // TODO: store as environment variable
		public string FromName { get; set; }
		public string FromAddress { get; set; }
	}
}