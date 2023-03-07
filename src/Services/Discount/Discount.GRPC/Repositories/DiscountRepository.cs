using Dapper;
using Discount.GRPC.Entities;
using Discount.GRPC.Options;
using Discount.GRPC.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Discount.GRPC.Repositories;

public class DiscountRepository : IDiscountRepository
{
	private readonly IOptions<NpgsqlOptions> _options;

	public DiscountRepository(IOptions<NpgsqlOptions> options)
	{
		_options = options;
	}

	public async Task<Coupon> GetDiscountAsync(string productName)
	{
		await using (var connection = new NpgsqlConnection(_options.Value.ConnectionString))
		{
			var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>
				("SELECT * FROM Coupon WHERE ProductName = @ProductName", new { ProductName = productName });

			if (coupon is null)
			{
				return new Coupon(string.Empty, string.Empty, 0);
			}

			return coupon;
		}
	}

	public async Task<bool> CreateDiscountAsync(Coupon coupon)
	{
		await using (var connection = new NpgsqlConnection(_options.Value.ConnectionString))
		{
			var affected = await connection.ExecuteAsync
			("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
				new { coupon.ProductName, coupon.Description, coupon.Amount });


			return affected != 0;
		}
	}

	public async Task<bool> UpdateDiscountAsync(Coupon coupon)
	{
		await using (var connection = new NpgsqlConnection(_options.Value.ConnectionString))
		{
			var affected = await connection.ExecuteAsync
			("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount WHERE Id == @Id",
				new { coupon.ProductName, coupon.Description, coupon.Amount, coupon.Id });

			return affected != 0;
		}
	}

	public async Task<bool> DeleteDiscountAsync(string productName)
	{
		await using (var connection = new NpgsqlConnection(_options.Value.ConnectionString))
		{
			var affected = await connection.ExecuteAsync
				("DELETE FROM Coupon WHERE ProductName == @ProductName", new { ProductName = productName });

			return affected != 0;
		}
	}
}