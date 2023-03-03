using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
	internal class CheckoutOrderCommandHandler : IRequestHandler<CheckoutOrderCommand, Guid>
	{
		private const string To = "a.kudryavcev8080@gmail.com";
		private const string Subject = "Order was created";
		private const string Body = "Hello, order was created successfully, please continue on payment page";
		private readonly IOrderRepository _orderRepository;
		private readonly IMapper _mapper;
		private readonly IEmailService _emailService;

		public CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, IEmailService emailService)

		{
			_orderRepository = orderRepository;
			_mapper = mapper;
			_emailService = emailService;
		}

		public async Task<Guid> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
		{
			var orderEntity = _mapper.Map<Order>(request);
			var newOrder = await _orderRepository.AddAsync(orderEntity);
			var email = new EmailModel(To, Subject, Body);

			if (!await _emailService.SendEmailAsync(email))
			{
				// TODO: Log error
			}

			return newOrder.Id;
		}
	}
}