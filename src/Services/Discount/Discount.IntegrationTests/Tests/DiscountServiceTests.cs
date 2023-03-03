using AutoFixture;
using AutoMapper;
using Discount.GRPC;
using Discount.GRPC.Entities;
using Discount.GRPC.Options;
using Discount.GRPC.Repositories.Interfaces;
using Discount.IntegrationTests.Factory;
using Discount.IntegrationTests.Helpers;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace Discount.IntegrationTests.Tests
{
	public class DiscountServiceTests : IDisposable
	{
		private readonly NpgsqlOptions _options;
		private readonly Mock<IMapper> _mapperMock;
		private readonly Mock<IDiscountRepository> _discountRepositoryMock;
		private readonly Mock<IOptions<NpgsqlOptions>> _npgsqlOptionsMock;
		private readonly Action<IServiceCollection> _configureMockServices;

		private DiscountWebApplicationFactory<Program> _factory = null!;
		private DiscountProto.DiscountProtoClient _client = null!;

		public DiscountServiceTests()
		{
			_options = new NpgsqlOptions()
			{
				ConnectionString = "Server=localhost;Port=5432;Database=TestDiscountDb;User Id=postgres;Password=PostgresPassword;"
			};
			_mapperMock = new Mock<IMapper>();
			_discountRepositoryMock = new Mock<IDiscountRepository>();
			_npgsqlOptionsMock = new Mock<IOptions<NpgsqlOptions>>();
			_configureMockServices = services =>
			{
				services.AddSingleton<IMapper>(_mapperMock.Object);
				services.AddSingleton<IDiscountRepository>(_discountRepositoryMock.Object);
				services.AddSingleton<IOptions<NpgsqlOptions>>(_npgsqlOptionsMock.Object);
			};
		}

		public GrpcChannel GrpcChannel { get; set; } = null!;

		[Fact]
		public async Task GetDiscountAsync_ShouldReturnDiscount()
		{
			// arrange
			var coupon = FakeStorage.GetCoupon();
			var couponResponse = FakeStorage.GetCouponResponse();
			var request = new GetDiscountRequest() { ProductName = coupon.ProductName };

			_mapperMock.Setup(s => s.Map<CouponResponse>(coupon)).Returns(couponResponse);
			_discountRepositoryMock.Setup(s => s.GetDiscountAsync(request.ProductName)).ReturnsAsync(coupon);
			_npgsqlOptionsMock.Setup(s => s.Value).Returns(_options);

			Configure(_configureMockServices);

			// act
			var response = await _client.GetDiscountAsync(request);

			// assert
			Assert.NotNull(response);
			Assert.Equal(couponResponse.ProductName, response.ProductName);
			Assert.Equal(couponResponse.Description, response.Description);
			Assert.Equal(couponResponse.Amount, response.Amount);

			_mapperMock.Verify(v => v.Map<CouponResponse>(It.IsAny<Coupon>()), Times.Once);
			_discountRepositoryMock.Verify(v => v.GetDiscountAsync(It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task CreateDiscountAsync_ShouldCreateDiscount()
		{
			// arrange
			var command = new Fixture().Create<CreateDiscountCommand>();
			var coupon = MappingHelper.MapCouponFromCreateDiscountCommand(command);
			var couponResponse = MappingHelper.MapCouponResponseFromCoupon(coupon);

			_mapperMock.Setup(s => s.Map<Coupon>(command)).Returns(coupon);
			_mapperMock.Setup(s => s.Map<CouponResponse>(coupon)).Returns(couponResponse);
			_discountRepositoryMock.Setup(s => s.CreateDiscountAsync(coupon)).ReturnsAsync(true);
			_discountRepositoryMock.Setup(s => s.GetDiscountAsync(command.ProductName)).ReturnsAsync(coupon);
			_npgsqlOptionsMock.Setup(s => s.Value).Returns(_options);

			Configure(_configureMockServices);

			// act
			var response = await _client.CreateDiscountAsync(command);
			var createdCouponResponse = await _client.GetDiscountAsync(new GetDiscountRequest() { ProductName = command.ProductName });

			Assert.NotNull(response);
			Assert.True(response.Success);
			Assert.NotNull(createdCouponResponse);
			Assert.Equal(command.ProductName, createdCouponResponse.ProductName);

			_mapperMock.Verify(v => v.Map<Coupon>(It.IsAny<CreateDiscountCommand>()), Times.Once);
			_mapperMock.Verify(v => v.Map<CouponResponse>(It.IsAny<Coupon>()), Times.Once);
			_discountRepositoryMock.Verify(v => v.CreateDiscountAsync(It.IsAny<Coupon>()), Times.Once);
			_discountRepositoryMock.Verify(v => v.GetDiscountAsync(It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task UpdateDiscountAsync_ShouldUpdateDiscount()
		{
			// arrange
			var command = new Fixture().Build<UpdateDiscountCommand>()
				.Without(prop => prop.Id)
				.Create();
			command.Id = 1;
			var coupon = MappingHelper.MapCouponFromUpdateDiscountCommand(command);
			var couponResponse = MappingHelper.MapCouponResponseFromCoupon(coupon);

			_mapperMock.Setup(s => s.Map<Coupon>(command)).Returns(coupon);
			_mapperMock.Setup(s => s.Map<CouponResponse>(coupon)).Returns(couponResponse);
			_discountRepositoryMock.Setup(s => s.UpdateDiscountAsync(coupon)).ReturnsAsync(true);
			_discountRepositoryMock.Setup(s => s.GetDiscountAsync(command.ProductName)).ReturnsAsync(coupon);
			_npgsqlOptionsMock.Setup(s => s.Value).Returns(_options);

			Configure(_configureMockServices);

			// act
			var response = await _client.UpdateDiscountAsync(command);
			var updatedDiscountResponse = await _client.GetDiscountAsync(new GetDiscountRequest() { ProductName = command.ProductName });

			// assert
			Assert.NotNull(response);
			Assert.True(response.Success);
			Assert.NotNull(updatedDiscountResponse);
			Assert.Equal(command.ProductName, updatedDiscountResponse.ProductName);

			_mapperMock.Verify(v => v.Map<Coupon>(It.IsAny<UpdateDiscountCommand>()), Times.Once);
			_mapperMock.Verify(v => v.Map<CouponResponse>(It.IsAny<Coupon>()), Times.Once);
			_discountRepositoryMock.Verify(v => v.UpdateDiscountAsync(It.IsAny<Coupon>()), Times.Once);
			_discountRepositoryMock.Verify(v => v.GetDiscountAsync(It.IsAny<string>()), Times.Once);
		}

		[Fact]
		public async Task DeleteDiscountAsync_ShouldDeleteDiscount()
		{
			// arrange
			var command = new DeleteDiscountCommand() { ProductName = FakeStorage.GetCoupon().ProductName };

			_discountRepositoryMock.Setup(s => s.DeleteDiscountAsync(command.ProductName)).ReturnsAsync(true);
			_discountRepositoryMock.Setup(s => s.GetDiscountAsync(command.ProductName)).ThrowsAsync(It.IsAny<RpcException>());
			_npgsqlOptionsMock.Setup(s => s.Value).Returns(_options);

			Configure(_configureMockServices);

			// act
			var response = await _client.DeleteDiscountAsync(command);

			// assert
			Assert.NotNull(response);
			Assert.True(response.Success);
			await Assert.ThrowsAsync<RpcException>(
				async () => await _client.GetDiscountAsync(new GetDiscountRequest() { ProductName = command.ProductName }));
		}

		public void Dispose()
		{
			_factory.Dispose();
		}

		private void Configure(Action<IServiceCollection> configure)
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