using MassTransit;
using MassTransit.Contracts.Common;
using Ordering.API.EventBusConsumers;
using Ordering.API.Extensions;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Extensions;
using Ordering.Infrastructure.Mail;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddMassTransit(options =>
{
	options.AddConsumer<BasketCheckoutConsumer>();

	options.UsingRabbitMq((context, config) =>
	{
		config.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
		{
			c.ConfigureConsumer<BasketCheckoutConsumer>(context);
		});
	});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.MigrateDatabaseAsync();

app.Run();