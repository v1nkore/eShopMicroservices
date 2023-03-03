using Microsoft.Data.SqlClient;
using Ordering.Infrastructure.Helpers;
using Polly;

namespace Ordering.API.Extensions
{
	public static class ServiceProviderExtensions
	{
		public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
		{
			await using (var scope = serviceProvider.CreateAsyncScope())
			{
				var retry = Policy.Handle<SqlException>()
					.WaitAndRetryAsync(
						retryCount: 5,
						sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
						onRetry: (exception, retryCount, context) =>
						{
							// TODO: Logging
							Console.WriteLine($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}");
						});

				await retry.ExecuteAsync(async () => await serviceProvider.MigrateAsync());
			}
		}
	}
}
