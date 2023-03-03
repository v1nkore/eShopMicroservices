using MediatR;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
	public class UpdateOrderCommand : IRequest<Order?>
	{
		public Guid Id { get; set; }
		public string UserName { get; set; } = null!;
		public decimal TotalPrice { get; set; }

		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public string EmailAddress { get; set; } = null!;
		public string? AddressLine { get; set; }
		public string Country { get; set; } = null!;
		public string? State { get; set; }
		public string? ZipCode { get; set; }

		public string? CardName { get; set; }
		public string? CardNumber { get; set; }
		public string? Expiration { get; set; }
		public string? Cvv { get; set; }
		public int? PaymentMethod { get; set; }
	}
}