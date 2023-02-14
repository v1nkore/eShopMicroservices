using AutoMapper;
using Discount.GRPC.Data;
using Discount.GRPC.Entities;
using Discount.GRPC.Repositories.Interfaces;
using Discount.IntegrationTests.Factory;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Discount.IntegrationTests.Tests
{
	public class DiscountServiceTests : IDisposable
	{
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IDiscountRepository> _discountRepositoryMock;
		private readonly Mock<IOptions<NpgsqlOptions>> _npgsqlOptionsMock;
		private DiscountWebApplicationFactory<Program> _factory = null!;
		private DiscountProto.DiscountProtoClient _client = null!;

		public DiscountServiceTests()
		{
			_mapperMock = new Mock<IMapper>();
			_discountRepositoryMock = new Mock<IDiscountRepository>();
			_npgsqlOptionsMock = new Mock<IOptions<NpgsqlOptions>>();
		}

		public GrpcChannel GrpcChannel { get; set; } = null!;

		[Fact]
		public async Task GetDiscountAsync_ShouldReturnDiscount()
		{
			// arrange
			var options = new NpgsqlOptions()
			{
				ConnectionString = "Server=localhost;Port=5432;Database=TestDiscountDb;User Id=postgres;Password=PostgresPassword;"
			};
			var request = new GetDiscountRequest() { ProductName = "Product" };
			var coupon = new Coupon() { ProductName = "Product", Description = "ProductDiscount", Amount = 20 };
			var expected = new CouponResponse() { ProductName = "Product", Description = "ProductDiscount", Amount = 20 };

			_discountRepositoryMock.Setup(s => s.GetDiscountAsync(request.ProductName)).ReturnsAsync(coupon);
			_npgsqlOptionsMock.Setup(s => s.Value).Returns(options);

			ConfigureFactoryServices(services =>
			{
				services.AddSingleton(_discountRepositoryMock.Object);
				services.AddSingleton(_npgsqlOptionsMock.Object);
			});

			// act
			var response = await _client.GetDiscountAsync(request);

			// assert
			Assert.NotNull(response);
			Assert.Equal(expected.ProductName, response.ProductName);
			Assert.Equal(expected.Description, response.Description);
			Assert.Equal(expected.Amount, response.Amount);
		}

		[Fact]
		public async Task Test()
		{
			// arrange
			const string expected = "Test response message";
			ConfigureFactoryServices(null);
			var client = new TestProto.TestProtoClient(GrpcChannel);

			// act
			var response = await client.TestGetStringAsync(new Empty());

			// assert
			Assert.Equal(expected, response.ResponseMessage);
		}

		public void Dispose()
		{
			_factory.Dispose();
		}

		private void ConfigureFactoryServices(Action<IServiceCollection> configure)
		{
			_factory = new DiscountWebApplicationFactory<Program>(configure);
			var httpClient = _factory.CreateDefaultClient(new ResponseVersionHandler());
			GrpcChannel = GrpcChannel.ForAddress(httpClient.BaseAddress, new GrpcChannelOptions()
			{
				HttpClient = httpClient,
			});
			_client = new DiscountProto.DiscountProtoClient(GrpcChannel);
		}

		private class ResponseVersionHandler : DelegatingHandler
		{
			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				var response = await base.SendAsync(request, cancellationToken);
				response.Version = request.Version;

				return response;
			}
		}
	}
}