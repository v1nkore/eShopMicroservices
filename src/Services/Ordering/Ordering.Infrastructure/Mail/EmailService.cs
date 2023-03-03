using Microsoft.Extensions.Options;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Models;
using Ordering.Application.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Ordering.Infrastructure.Mail
{
	public class EmailService : IEmailService
	{
		private readonly SendGridClient _emailClient;
		private readonly IOptions<EmailOptions> _emailOptions;

		public EmailService(SendGridClient emailClient, IOptions<EmailOptions> emailOptions)
		{
			_emailClient = emailClient;
			_emailOptions = emailOptions;
		}

		public async Task<bool> SendEmailAsync(EmailModel email)
		{
			var subject = email.Subject;
			var to = new EmailAddress(email.To);
			var emailBody = email.Body;

			var from = new EmailAddress()
			{
				Email = _emailOptions.Value.FromAddress,
				Name = _emailOptions.Value.FromName
			};

			var sendGridMessage = MailHelper.CreateSingleEmail(from, to, subject, emailBody, emailBody);
			var response = await _emailClient.SendEmailAsync(sendGridMessage);

			return response.IsSuccessStatusCode;
		}
	}
}