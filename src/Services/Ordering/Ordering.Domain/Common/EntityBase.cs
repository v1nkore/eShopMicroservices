namespace Ordering.Domain.Common
{
	public abstract class EntityBase
	{
		public Guid Id { get; protected set; }
		public string CreatedBy { get; set; } = null!;
		public DateTime CreatedDate { get; set; }
		public string? LastModifiedBy { get; set; }
		public DateTime? LastModifiedDate { get; set; }
	}
}