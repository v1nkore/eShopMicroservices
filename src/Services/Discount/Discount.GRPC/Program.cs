using Discount.GRPC.Data;
using Discount.GRPC.Extensions;
using Discount.GRPC.Repositories;
using Discount.GRPC.Repositories.Interfaces;
using Discount.GRPC.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHealthChecks()
	.AddNpgSql(
		builder.Configuration.GetConnectionString("NpgsqlSettings:ConnectionString"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.Configure<NpgsqlOptions>(builder.Configuration.GetSection(NpgsqlOptions.Section));
builder.Services.AddSingleton<INpgsqlOptions, NpgsqlOptions>();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapGrpcService<DiscountService>();
});
app.MapHealthChecks("hc", new HealthCheckOptions()
{
	Predicate = _ => true,
	ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

await app.Services.MigrateDatabaseAsync<Program>();

app.Run();
