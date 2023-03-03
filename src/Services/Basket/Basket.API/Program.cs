using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Basket.API.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Configs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddStackExchangeRedisCache(options => { options.Configuration = CacheConfig.ConnectionString; });

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<DiscountGrpcService>();

builder.Services.AddGrpcClient<DiscountProto.DiscountProtoClient>(options => { options.Address = new Uri(DiscountGrpcConfig.DiscountUrl); });

builder.Services.AddMassTransit();

builder.Services.AddHealthChecks()
	.AddRedis(CacheConfig.ConnectionString, "Redis Health", HealthStatus.Degraded);

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