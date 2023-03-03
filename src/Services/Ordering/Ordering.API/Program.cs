using EventBus.Messages.Common;
using MassTransit;
using Ordering.API.EventBusConsumers;
using Ordering.API.Extensions;
using Ordering.Application.Extensions;
using Ordering.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

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