using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Discount.IntegrationTests.Factory
{
	public class DiscountWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : Program
	{
		private readonly Action<IServiceCollection> _configureWebHost;

		public DiscountWebApplicationFactory(Action<IServiceCollection> configure)
		{
			_configureWebHost = configure;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			const string connectionString = "Server=localhost;Port=5432;Database=TestDiscountDb;User Id=postgres;Password=PostgresPassword;";
			builder.UseEnvironment(EnvironmentExtensions.IntegrationTestingEnvironmentName);

			builder.ConfigureTestServices(services =>
			{
				using (var scope = services.BuildServiceProvider().CreateScope())
				{
					using (var connection = new NpgsqlConnection(connectionString))
					{
						connection.Open();
						using (var command = new NpgsqlCommand() { Connection = connection })
						{
							command.CommandText = "DROP TABLE IF EXISTS Coupon";
							command.ExecuteNonQuery();

							command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY,
											ProductName VARCHAR(40) NOT NULL,
											Description TEXT,
											Amount INT);";
							command.ExecuteNonQuery();

							command.CommandText = @"INSERT INTO Coupon 
											(ProductName, Description, Amount) 
											VALUES('Product', 'ProductDiscount', '20');";
							command.ExecuteNonQuery();
						}

						connection.Close();
					}
				}

				_configureWebHost?.Invoke(services);
			});

			base.ConfigureWebHost(builder);
		}
	}
}