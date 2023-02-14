using Discount.GRPC.Data;
using Discount.GRPC.Repositories;
using Discount.GRPC.Repositories.Interfaces;
using Discount.GRPC.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString($"{NpgsqlOptions.Section}:ConnectionString"));

builder.Services.Configure<NpgsqlOptions>(builder.Configuration.GetSection(NpgsqlOptions.Section));
builder.Services.AddSingleton<INpgsqlOptions, NpgsqlOptions>();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.WebHost.ConfigureKestrel(options =>
{
	options.Listen(IPAddress.Any, 5167,
		listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
});

var app = builder.Build();

app.UseRouting();

app.MapGrpcService<DiscountService>();
app.MapGrpcService<TestService>();

app.MapGet("/",
	() =>
		"Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program
{
}