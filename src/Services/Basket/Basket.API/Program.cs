using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using MassTransit.Contracts.Configuration;
using MassTransit.Contracts.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = builder.Configuration.GetValue<string>("RedisCacheOptions:ConnectionString");
});

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<DiscountGrpcService>();

builder.Services.AddGrpcClient<DiscountProto.DiscountProtoClient>(options =>
{
	options.Address = new Uri(builder.Configuration.GetValue<string>("DiscountGrpcOptions:DiscountUrl"));
});

var section = builder.Configuration.GetSection("EventBusOptions");
NullCheckExtensions.ThrowIfNull(section);
MassTransitConfigurator.ConfigureServices(builder.Services, builder.Configuration, new MassTransitConfiguration()
{
	IsDebug = section.GetValue<bool>("EventBusOptions:IsDebug")
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

app.Run();