using Basket.API.Repositories;
using Basket.UnitTests.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;
using MemoryDistributedCacheOptions = Microsoft.Extensions.Caching.Memory.MemoryDistributedCacheOptions;

namespace Basket.UnitTests.Tests
{
	public class BasketRepositoryTests
	{
		private const string UserName = "UserName";
		private readonly BasketRepository _basketRepository;

		public BasketRepositoryTests()
		{
			var options = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
			var cache = new MemoryDistributedCache(options);
			cache.SetString(UserName, JsonSerializer.Serialize(FakeStorage.GetShoppingCart()));
			_basketRepository = new BasketRepository(cache);
		}

		[Fact]
		public async Task GetBasketAsync_ShouldReturnBasket_WhenItsCached()
		{
			// arrange

			// act
			var cachedBasket = await _basketRepository.GetBasketAsync(UserName);

			// assert
			Assert.NotNull(cachedBasket);
			Assert.Equal(cachedBasket!.UserName, cachedBasket.UserName);
			Assert.Equal(cachedBasket.TotalPrice, cachedBasket.TotalPrice);
		}

		[Fact]
		public async Task UpdateBasketAsync_ShouldReturnBasket_WhenItGetsUpdated()
		{
			// arrange
			var basket = FakeStorage.GetUpdatedShoppingCart();

			// act
			var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

			// assert
			Assert.NotNull(updatedBasket);
			Assert.Equal(updatedBasket!.UserName, basket.UserName);
			Assert.Equal(updatedBasket.TotalPrice, basket.TotalPrice);
		}

		[Fact]
		public async Task DeleteBasketAsync_ShouldDeleteBasket()
		{
			// arrange

			// act
			await _basketRepository.DeleteBasketAsync(UserName);
			var deletedBasket = await _basketRepository.GetBasketAsync(UserName);

			// assert
			Assert.Null(deletedBasket);
		}

		[Fact]
		public async Task GetBasketAsync_ShouldReturnNull_WhenBasketIsNotCached()
		{
			// arrange
			const string userName = "";

			// act
			var cachedBasket = await _basketRepository.GetBasketAsync(userName);

			// assert
			Assert.Null(cachedBasket);
		}
	}
}
