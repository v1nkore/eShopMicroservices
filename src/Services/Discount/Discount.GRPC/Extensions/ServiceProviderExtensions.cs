using Discount.GRPC.Options;
using Microsoft.Extensions.Options;
using Npgsql;
using Polly;

namespace Discount.GRPC.Extensions;

public static class ServiceProviderExtensions
{
	public static async Task MigrateDatabaseAsync<TContext>(this IServiceProvider serviceProvider)
	{
		await using (var scope = serviceProvider.CreateAsyncScope())
		{
			var options = scope.ServiceProvider.GetRequiredService<IOptions<NpgsqlOptions>>();

			var retry = Policy.Handle<NpgsqlException>()
				.WaitAndRetryAsync(
					retryCount: 5,
					sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
					onRetry: (exception, retryCount, context) =>
					{
						// TODO: Logging
						Console.WriteLine($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}");
					});

			await retry.ExecuteAsync(async () => await ExecuteMigrations(options.Value.ConnectionString));
		}
	}

	private static async Task ExecuteMigrations(string connectionString)
	{
		await using (var connection = new NpgsqlConnection(connectionString))
		{
			await connection.OpenAsync();

			await using (var command = new NpgsqlCommand() { Connection = connection })
			{
				command.CommandText = "DROP TABLE IF EXISTS Coupon";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
											ProductName VARCHAR(40) NOT NULL,
											Description TEXT,
											Amount INT);";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"INSERT INTO Coupon 
											(ProductName, Description, Amount) 
											VALUES('MacBook Air 13', 'MacBook Air Discount', '200');";
				await command.ExecuteNonQueryAsync();

				command.CommandText = @"INSERT INTO Coupon 
											(ProductName, Description, Amount) 
											VALUES('Lenovo Legion 5 Pro', 'Lenovo Legion Discount', '242')";

				command.CommandText = @"INSERT INTO Coupon 
											(ProductName, Description, Amount) 
											VALUES('Product', 'ProductDiscount', '20');";
				await command.ExecuteNonQueryAsync();
			}
		}
	}
}