namespace MassTransit.Contracts.Commands
{
	public class IntegrationCommandBase
	{
		public IntegrationCommandBase()
		{
			Id = Guid.NewGuid();
			CreatedAt = DateTime.UtcNow;
		}

		public IntegrationCommandBase(Guid id, DateTime createdAt)
		{
			Id = id;
			CreatedAt = createdAt;
		}

		public Guid Id { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}